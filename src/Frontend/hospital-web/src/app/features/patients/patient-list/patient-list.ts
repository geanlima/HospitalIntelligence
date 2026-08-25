import { CommonModule } from '@angular/common';
import {
  Component,
  inject,
  signal
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { Patient } from '../../../core/models/patient.model';
import { PatientService } from '../../../core/services/patient.service';

@Component({
  selector: 'app-patient-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule
  ],
  templateUrl: './patient-list.html',
  styleUrl: './patient-list.scss'
})
export class PatientList {
  private readonly patientService = inject(PatientService);
  private readonly router = inject(Router);

  readonly patients = signal<Patient[]>([]);
  readonly loading = signal(false);
  readonly errorMessage = signal('');
  readonly hasSearched = signal(false);

  searchName = '';
  externalSystem = '';

  currentPage = 1;
  pageSize = 10;

  search(): void {
    const name = this.searchName.trim();
    const sourceSystem = this.externalSystem.trim();

    this.loading.set(true);
    this.errorMessage.set('');
    this.hasSearched.set(true);

    this.patientService
      .getPatients(
        name,
        sourceSystem
      )
      .subscribe({
        next: patients => {
          this.patients.set(patients);
          this.currentPage = 1;
          this.loading.set(false);
        },

        error: error => {
          console.error(
            'Erro ao buscar pacientes:',
            error
          );

          this.patients.set([]);

          this.errorMessage.set(
            'Não foi possível buscar os pacientes.'
          );

          this.loading.set(false);
        }
      });
  }

  clearSearch(): void {
    this.searchName = '';
    this.externalSystem = '';

    this.patients.set([]);
    this.errorMessage.set('');
    this.loading.set(false);
    this.hasSearched.set(false);

    this.currentPage = 1;
  }

  get paginatedPatients(): Patient[] {
    const start =
      (this.currentPage - 1) * this.pageSize;

    const end =
      start + this.pageSize;

    return this.patients().slice(
      start,
      end
    );
  }

  get totalPages(): number {
    return Math.ceil(
      this.patients().length /
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
}