import { CommonModule } from '@angular/common';
import {
  Component,
  inject,
  signal
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { Prescription } from '../../../core/models/prescription.model';
import { PrescriptionService } from '../../../core/services/prescription.service';

@Component({
  selector: 'app-prescription-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule
  ],
  templateUrl: './prescription-list.html',
  styleUrl: './prescription-list.scss'
})
export class PrescriptionListComponent {
  private readonly prescriptionService =
    inject(PrescriptionService);

  private readonly router = inject(Router);

  readonly prescriptions = signal<Prescription[]>([]);
  readonly loading = signal(false);
  readonly errorMessage = signal('');
  readonly hasSearched = signal(false);

  status = '';

  currentPage = 1;
  pageSize = 10;

  search(): void {
    this.loading.set(true);
    this.errorMessage.set('');
    this.hasSearched.set(true);

    this.prescriptionService
      .getPrescriptions(this.status)
      .subscribe({
        next: prescriptions => {
          this.prescriptions.set(prescriptions);
          this.currentPage = 1;
          this.loading.set(false);
        },
        error: error => {
          console.error(
            'Erro ao buscar prescrições:',
            error
          );
          this.prescriptions.set([]);
          this.errorMessage.set(
            'Não foi possível buscar as prescrições.'
          );
          this.loading.set(false);
        }
      });
  }

  clearSearch(): void {
    this.status = '';
    this.prescriptions.set([]);
    this.errorMessage.set('');
    this.loading.set(false);
    this.hasSearched.set(false);
    this.currentPage = 1;
  }

  get paginatedPrescriptions(): Prescription[] {
    const start = (this.currentPage - 1) * this.pageSize;
    return this.prescriptions().slice(
      start,
      start + this.pageSize
    );
  }

  get totalPages(): number {
    return Math.ceil(
      this.prescriptions().length / this.pageSize
    );
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
        return 'Ativa';
      case 'Suspended':
        return 'Suspensa';
      case 'Completed':
        return 'Concluída';
      case 'Cancelled':
        return 'Cancelada';
      default:
        return status;
    }
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'Active':
        return 'status-active';
      case 'Suspended':
        return 'status-suspended';
      case 'Completed':
        return 'status-completed';
      case 'Cancelled':
        return 'status-cancelled';
      default:
        return 'status-neutral';
    }
  }
}
