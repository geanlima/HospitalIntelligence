import { CommonModule } from '@angular/common';
import {
  Component,
  inject,
  signal
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { Admission } from '../../../core/models/admission.model';
import { AdmissionService } from '../../../core/services/admission.service';

@Component({
  selector: 'app-admission-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule
  ],
  templateUrl: './admission-list.html',
  styleUrl: './admission-list.scss'
})
export class AdmissionListComponent {
  private readonly admissionService =
    inject(AdmissionService);

  private readonly router =
    inject(Router);

  readonly admissions =
    signal<Admission[]>([]);

  readonly loading =
    signal(false);

  readonly errorMessage =
    signal('');

  readonly hasSearched =
    signal(false);

  status = '';
  unit = '';

  currentPage = 1;
  pageSize = 10;

  search(): void {
    this.loading.set(true);
    this.errorMessage.set('');
    this.hasSearched.set(true);

    this.admissionService
      .getAdmissions(
        this.status,
        this.unit
      )
      .subscribe({
        next: admissions => {
          this.admissions.set(admissions);
          this.currentPage = 1;
          this.loading.set(false);
        },
        error: error => {
          console.error(
            'Erro ao buscar internações:',
            error
          );

          this.admissions.set([]);

          this.errorMessage.set(
            'Não foi possível buscar as internações.'
          );

          this.loading.set(false);
        }
      });
  }

  clearSearch(): void {
    this.status = '';
    this.unit = '';

    this.admissions.set([]);
    this.errorMessage.set('');
    this.loading.set(false);
    this.hasSearched.set(false);

    this.currentPage = 1;
  }

  get paginatedAdmissions(): Admission[] {
    const start =
      (this.currentPage - 1) *
      this.pageSize;

    const end =
      start +
      this.pageSize;

    return this.admissions()
      .slice(start, end);
  }

  get totalPages(): number {
    return Math.ceil(
      this.admissions().length /
      this.pageSize
    );
  }

  get pages(): number[] {
    return Array.from(
      {
        length: this.totalPages
      },
      (_, index) => index + 1
    );
  }

  goToPage(page: number): void {
    if (
      page < 1 ||
      page > this.totalPages
    ) {
      return;
    }

    this.currentPage = page;
  }

  previousPage(): void {
    this.goToPage(
      this.currentPage - 1
    );
  }

  nextPage(): void {
    this.goToPage(
      this.currentPage + 1
    );
  }

  viewPatient360(
    patientId: string
  ): void {
    this.router.navigate([
      '/patients',
      patientId,
      '360'
    ]);
  }

  formatStatus(status: string): string {
    switch (status) {
      case 'Active':
        return 'Ativa';

      case 'Discharged':
        return 'Alta';

      case 'Cancelled':
        return 'Cancelada';

      default:
        return status;
    }
  }

  getStatusClass(
    status: string
  ): string {
    switch (status) {
      case 'Active':
        return 'status-active';

      case 'Discharged':
        return 'status-discharged';

      case 'Cancelled':
        return 'status-cancelled';

      default:
        return 'status-neutral';
    }
  }
}
