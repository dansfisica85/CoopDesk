import { CommonModule } from '@angular/common';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { firstValueFrom } from 'rxjs';

type TicketPriority = 'Low' | 'Medium' | 'High' | 'Critical';
type TicketStatus = 'Open' | 'InProgress' | 'WaitingBusiness' | 'Resolved' | 'Closed' | 'Canceled';

interface LookupItem {
  id: string;
  name: string;
}

interface TicketSummary {
  id: string;
  title: string;
  priority: TicketPriority;
  status: TicketStatus;
  requesterName: string;
  departmentName: string;
  assignedAgentName?: string | null;
  createdAtUtc: string;
  dueAtUtc?: string | null;
}

@Component({
  selector: 'app-root',
  imports: [CommonModule, FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App implements OnInit {
  private readonly http = inject(HttpClient);

  apiBase = 'http://localhost:5298';
  loading = false;
  message = 'Pronto';

  readonly priorities: TicketPriority[] = ['Low', 'Medium', 'High', 'Critical'];
  readonly statuses: TicketStatus[] = ['Open', 'InProgress', 'WaitingBusiness', 'Resolved', 'Closed', 'Canceled'];

  tickets: TicketSummary[] = [];
  departments: LookupItem[] = [];
  collaborators: LookupItem[] = [];

  filters = {
    status: '',
    priority: '',
    departmentId: '',
    search: ''
  };

  newTicket = {
    title: '',
    description: '',
    priority: 'Medium' as TicketPriority,
    requesterId: '',
    departmentId: ''
  };

  ngOnInit(): void {
    void this.loadReferenceData();
    void this.loadTickets();
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

  async loadReferenceData(): Promise<void> {
    try {
      const [departments, collaborators] = await Promise.all([
        firstValueFrom(this.http.get<LookupItem[]>(this.url('api/reference-data/departments'))),
        firstValueFrom(this.http.get<LookupItem[]>(this.url('api/reference-data/collaborators')))
      ]);

      this.departments = departments ?? [];
      this.collaborators = collaborators ?? [];
      this.newTicket.departmentId = this.departments[0]?.id ?? '';
      this.newTicket.requesterId = this.collaborators[0]?.id ?? '';
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

      if (this.filters.departmentId) {
        params = params.set('departmentId', this.filters.departmentId);
      }

      if (this.filters.search.trim()) {
        params = params.set('search', this.filters.search.trim());
      }

      this.tickets = await firstValueFrom(this.http.get<TicketSummary[]>(this.url('api/tickets'), { params }));
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
        priority: this.newTicket.priority,
        requesterId: this.newTicket.requesterId,
        departmentId: this.newTicket.departmentId,
        dueAtUtc: null
      }));

      this.newTicket.title = '';
      this.newTicket.description = '';
      this.message = 'Chamado criado.';
      await this.loadTickets();
    } catch (error) {
      this.setError('Nao foi possivel criar chamado.', error);
    }
  }

  async changeStatus(ticket: TicketSummary, status: TicketStatus): Promise<void> {
    try {
      await firstValueFrom(this.http.patch(this.url(`api/tickets/${ticket.id}/status`), {
        status,
        notes: `Status alterado pelo Angular para ${status}.`,
        performedBy: 'angular-web'
      }));

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

  private url(path: string): string {
    return `${this.apiBase.trim().replace(/\/$/, '')}/${path.replace(/^\//, '')}`;
  }

  private setError(message: string, error: unknown): void {
    console.error(message, error);
    this.message = message;
  }
}
