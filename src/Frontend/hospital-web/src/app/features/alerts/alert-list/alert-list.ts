import { CommonModule } from '@angular/common';
import {
  Component,
  inject,
  signal
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { Alert } from '../../../core/models/alert.model';
import { AlertService } from '../../../core/services/alert.service';

@Component({
  selector: 'app-alert-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule
  ],
  templateUrl: './alert-list.html',
  styleUrl: './alert-list.scss'
})
export class AlertListComponent {
  private readonly alertService = inject(AlertService);
  private readonly router = inject(Router);

  readonly alerts = signal<Alert[]>([]);
  readonly loading = signal(false);
  readonly errorMessage = signal('');
  readonly hasSearched = signal(false);

  status = '';
  severity = '';

  currentPage = 1;
  pageSize = 10;

  search(): void {
    this.loading.set(true);
    this.errorMessage.set('');
    this.hasSearched.set(true);

    this.alertService
      .getAlerts(this.status, this.severity)
      .subscribe({
        next: alerts => {
          this.alerts.set(alerts);
          this.currentPage = 1;
          this.loading.set(false);
        },
        error: error => {
          console.error('Erro ao buscar alertas:', error);
          this.alerts.set([]);
          this.errorMessage.set(
            'Não foi possível buscar os alertas.'
          );
          this.loading.set(false);
        }
      });
  }

  clearSearch(): void {
    this.status = '';
    this.severity = '';
    this.alerts.set([]);
    this.errorMessage.set('');
    this.loading.set(false);
    this.hasSearched.set(false);
    this.currentPage = 1;
  }

  get paginatedAlerts(): Alert[] {
    const start = (this.currentPage - 1) * this.pageSize;
    return this.alerts().slice(start, start + this.pageSize);
  }

  get totalPages(): number {
    return Math.ceil(this.alerts().length / this.pageSize);
  }

  get pages(): number[] {
    return Array.from(
      { length: this.totalPages },
      (_, index) => index + 1
    );
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages) {
      return;
    }

    this.currentPage = page;
  }

  previousPage(): void {
    this.goToPage(this.currentPage - 1);
  }

  nextPage(): void {
    this.goToPage(this.currentPage + 1);
  }

  viewPatient360(patientId: string): void {
    this.router.navigate(['/patients', patientId, '360']);
  }

  formatStatus(status: string): string {
    switch (status) {
      case 'Active':
        return 'Ativo';
      case 'Acknowledged':
        return 'Reconhecido';
      case 'Resolved':
        return 'Resolvido';
      default:
        return status;
    }
  }

  formatSeverity(severity: string): string {
    switch (severity) {
      case 'Low':
        return 'Baixa';
      case 'Medium':
        return 'Média';
      case 'High':
        return 'Alta';
      case 'Critical':
        return 'Crítica';
      default:
        return severity;
    }
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'Active':
        return 'status-active';
      case 'Acknowledged':
        return 'status-acknowledged';
      case 'Resolved':
        return 'status-resolved';
      default:
        return 'status-neutral';
    }
  }

  getSeverityClass(severity: string): string {
    switch (severity) {
      case 'Low':
        return 'status-low';
      case 'Medium':
        return 'status-medium';
      case 'High':
        return 'status-high';
      case 'Critical':
        return 'status-critical';
      default:
        return 'status-neutral';
    }
  }
}
