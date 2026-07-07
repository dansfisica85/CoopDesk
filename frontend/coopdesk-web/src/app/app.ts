import { CommonModule } from '@angular/common';
import { HttpClient, HttpErrorResponse, HttpHeaders, HttpParams } from '@angular/common/http';
import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { firstValueFrom } from 'rxjs';

type TicketPriority = 'Low' | 'Medium' | 'High' | 'Critical';
type TicketStatus = 'Open' | 'InProgress' | 'WaitingBusiness' | 'Resolved' | 'Closed' | 'Canceled';
type SupportProblemType = 'Access' | 'SystemError' | 'SlowPerformance' | 'RegistrationUpdate' | 'ReportIssue' | 'Other';
type UserRole = 'Administrator' | 'Agent' | 'Requester';

interface LookupItem {
  id: string;
  name: string;
}

interface TicketSummary {
  id: string;
  title: string;
  problemType: SupportProblemType;
  priority: TicketPriority;
  status: TicketStatus;
  requesterName: string;
  departmentName: string;
  assignedAgentName?: string | null;
  createdAtUtc: string;
  dueAtUtc?: string | null;
}

interface AuthenticatedUser {
  id: string;
  fullName: string;
  email: string;
  role: UserRole;
}

interface LoginResponse {
  accessToken: string;
  expiresAtUtc: string;
  user: AuthenticatedUser;
}

interface RuntimeConfig {
  apiBaseUrl?: string;
}

@Component({
  selector: 'app-root',
  imports: [CommonModule, FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App implements OnInit {
  private readonly http = inject(HttpClient);
  private readonly storageKey = 'coopdesk.auth';

  apiBase = 'http://localhost:5298';
  loading = false;
  message = 'Informe suas credenciais';
  authToken = '';
  currentUser?: AuthenticatedUser;

  readonly priorities: TicketPriority[] = ['Low', 'Medium', 'High', 'Critical'];
  readonly statuses: TicketStatus[] = ['Open', 'InProgress', 'WaitingBusiness', 'Resolved', 'Closed', 'Canceled'];
  readonly demoUsers = [
    { label: 'Admin', email: 'admin@coopdesk.local' },
    { label: 'Atendente', email: 'atendente@coopdesk.local' },
    { label: 'Solicitante', email: 'solicitante@coopdesk.local' }
  ];

  tickets: TicketSummary[] = [];
  departments: LookupItem[] = [];
  collaborators: LookupItem[] = [];
  problemTypes: { value: SupportProblemType; name: string }[] = [];

  loginForm = {
    email: 'atendente@coopdesk.local',
    password: 'Demo@12345'
  };

  filters = {
    status: '',
    priority: '',
    problemType: '',
    departmentId: '',
    search: ''
  };

  newTicket = {
    title: '',
    description: '',
    problemType: 'Other' as SupportProblemType,
    priority: 'Medium' as TicketPriority,
    requesterId: '',
    departmentId: ''
  };

  ngOnInit(): void {
    void this.bootstrapAsync();
  }

  private async bootstrapAsync(): Promise<void> {
    await this.loadRuntimeConfig();
    this.restoreSession();

    if (this.isAuthenticated) {
      void this.loadInitialData();
    }
  }

  get isAuthenticated(): boolean {
    return Boolean(this.authToken && this.currentUser);
  }

  get canManageTickets(): boolean {
    return this.currentUser?.role === 'Administrator' || this.currentUser?.role === 'Agent';
  }

  get openTickets(): number {
    return this.tickets.filter(ticket => ticket.status === 'Open').length;
  }

  get criticalTickets(): number {
    return this.tickets.filter(ticket => ticket.priority === 'Critical').length;
  }

  get inProgressTickets(): number {
    return this.tickets.filter(ticket => ticket.status === 'InProgress').length;
  }

  async login(): Promise<void> {
    if (!this.loginForm.email.trim() || !this.loginForm.password.trim()) {
      this.message = 'Informe e-mail e senha.';
      return;
    }

    this.loading = true;

    try {
      const response = await firstValueFrom(this.http.post<LoginResponse>(this.url('api/auth/login'), {
        email: this.loginForm.email,
        password: this.loginForm.password
      }));

      this.authToken = response.accessToken;
      this.currentUser = response.user;
      localStorage.setItem(this.storageKey, JSON.stringify(response));
      this.message = `Sessao iniciada como ${response.user.fullName}.`;
      await this.loadInitialData();
    } catch (error) {
      this.setError('E-mail ou senha invalidos.', error);
    } finally {
      this.loading = false;
    }
  }

  async useDemo(email: string): Promise<void> {
    this.loginForm.email = email;
    this.loginForm.password = 'Demo@12345';
    await this.login();
  }

  logout(): void {
    this.authToken = '';
    this.currentUser = undefined;
    this.tickets = [];
    this.departments = [];
    this.collaborators = [];
    localStorage.removeItem(this.storageKey);
    this.message = 'Sessao encerrada.';
  }

  async loadInitialData(): Promise<void> {
    await this.loadReferenceData();
    await this.loadTickets();
  }

  async loadReferenceData(): Promise<void> {
    try {
      const [departments, collaborators, problemTypes] = await Promise.all([
        firstValueFrom(this.http.get<LookupItem[]>(this.url('api/reference-data/departments'), { headers: this.authHeaders() })),
        firstValueFrom(this.http.get<LookupItem[]>(this.url('api/reference-data/collaborators'), { headers: this.authHeaders() })),
        firstValueFrom(this.http.get<{ value: SupportProblemType; name: string }[]>(this.url('api/reference-data/problem-types'), { headers: this.authHeaders() }))
      ]);

      this.departments = departments ?? [];
      this.collaborators = collaborators ?? [];
      this.problemTypes = problemTypes ?? [];
      this.newTicket.departmentId = this.departments[0]?.id ?? '';
      this.newTicket.requesterId = this.collaborators[0]?.id ?? '';
      this.newTicket.problemType = this.problemTypes[0]?.value ?? 'Other';
    } catch (error) {
      this.setError('Nao foi possivel carregar cadastros.', error);
    }
  }

  async loadTickets(): Promise<void> {
    this.loading = true;

    try {
      let params = new HttpParams();

      if (this.filters.status) {
        params = params.set('status', this.filters.status);
      }

      if (this.filters.priority) {
        params = params.set('priority', this.filters.priority);
      }

      if (this.filters.problemType) {
        params = params.set('problemType', this.filters.problemType);
      }

      if (this.filters.departmentId) {
        params = params.set('departmentId', this.filters.departmentId);
      }

      if (this.filters.search.trim()) {
        params = params.set('search', this.filters.search.trim());
      }

      this.tickets = await firstValueFrom(this.http.get<TicketSummary[]>(this.url('api/tickets'), { params, headers: this.authHeaders() }));
      this.message = `${this.tickets.length} chamado(s) carregado(s)`;
    } catch (error) {
      this.setError('Nao foi possivel carregar chamados.', error);
    } finally {
      this.loading = false;
    }
  }

  async createTicket(): Promise<void> {
    if (!this.newTicket.title.trim() || !this.newTicket.description.trim()) {
      this.message = 'Informe titulo e descricao.';
      return;
    }

    try {
      await firstValueFrom(this.http.post(this.url('api/tickets'), {
        title: this.newTicket.title,
        description: this.newTicket.description,
        problemType: this.newTicket.problemType,
        priority: this.newTicket.priority,
        requesterId: this.newTicket.requesterId,
        departmentId: this.newTicket.departmentId,
        dueAtUtc: null
      }, { headers: this.authHeaders() }));

      this.newTicket.title = '';
      this.newTicket.description = '';
      this.message = 'Chamado criado.';
      await this.loadTickets();
    } catch (error) {
      this.setError('Nao foi possivel criar chamado.', error);
    }
  }

  async changeStatus(ticket: TicketSummary, status: TicketStatus): Promise<void> {
    if (!this.canManageTickets) {
      this.message = 'Seu perfil permite abrir e consultar chamados.';
      return;
    }

    try {
      await firstValueFrom(this.http.patch(this.url(`api/tickets/${ticket.id}/status`), {
        status,
        notes: `Status alterado pelo Angular para ${status}.`,
        performedBy: this.currentUser?.fullName ?? 'angular-web'
      }, { headers: this.authHeaders() }));

      this.message = `Chamado atualizado para ${this.statusLabel(status)}.`;
      await this.loadTickets();
    } catch (error) {
      this.setError('Nao foi possivel atualizar chamado.', error);
    }
  }

  statusLabel(status: TicketStatus): string {
    const labels: Record<TicketStatus, string> = {
      Open: 'Aberto',
      InProgress: 'Em atendimento',
      WaitingBusiness: 'Aguardando area',
      Resolved: 'Resolvido',
      Closed: 'Fechado',
      Canceled: 'Cancelado'
    };

    return labels[status];
  }

  priorityLabel(priority: TicketPriority): string {
    const labels: Record<TicketPriority, string> = {
      Low: 'Baixa',
      Medium: 'Media',
      High: 'Alta',
      Critical: 'Critica'
    };

    return labels[priority];
  }

  problemTypeLabel(problemType: SupportProblemType): string {
    const labels: Record<SupportProblemType, string> = {
      Access: 'Acesso',
      SystemError: 'Erro',
      SlowPerformance: 'Lentidao',
      RegistrationUpdate: 'Cadastro',
      ReportIssue: 'Relatorio',
      Other: 'Outro'
    };

    return labels[problemType];
  }

  roleLabel(role?: UserRole): string {
    const labels: Record<UserRole, string> = {
      Administrator: 'Administrador',
      Agent: 'Atendente',
      Requester: 'Solicitante'
    };

    return role ? labels[role] : '';
  }

  private url(path: string): string {
    return `${this.apiBase.trim().replace(/\/$/, '')}/${path.replace(/^\//, '')}`;
  }

  private async loadRuntimeConfig(): Promise<void> {
    try {
      const config = await firstValueFrom(this.http.get<RuntimeConfig>('config.json'));
      if (config.apiBaseUrl?.trim()) {
        this.apiBase = config.apiBaseUrl.trim();
      }
    } catch {
      this.apiBase = 'http://localhost:5298';
    }
  }

  private authHeaders(): HttpHeaders {
    return new HttpHeaders({ Authorization: `Bearer ${this.authToken}` });
  }

  private restoreSession(): void {
    const raw = localStorage.getItem(this.storageKey);
    if (!raw) {
      return;
    }

    try {
      const session = JSON.parse(raw) as LoginResponse;
      if (!session.accessToken || new Date(session.expiresAtUtc).getTime() <= Date.now()) {
        this.logout();
        return;
      }

      this.authToken = session.accessToken;
      this.currentUser = session.user;
      this.message = `Sessao restaurada para ${session.user.fullName}.`;
    } catch {
      this.logout();
    }
  }

  private setError(message: string, error: unknown): void {
    console.error(message, error);

    if (error instanceof HttpErrorResponse && (error.status === 401 || error.status === 403)) {
      this.logout();
      this.message = 'Acesso negado ou sessao expirada.';
      return;
    }

    this.message = message;
  }
}
