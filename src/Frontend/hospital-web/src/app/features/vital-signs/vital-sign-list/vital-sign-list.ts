import { CommonModule } from '@angular/common';
import {
  Component,
  inject,
  signal
} from '@angular/core';
import { Router } from '@angular/router';

import { VitalSign } from '../../../core/models/vital-sign.model';
import { VitalSignService } from '../../../core/services/vital-sign.service';

@Component({
  selector: 'app-vital-sign-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './vital-sign-list.html',
  styleUrl: './vital-sign-list.scss'
})
export class VitalSignListComponent {
  private readonly vitalSignService =
    inject(VitalSignService);

  private readonly router = inject(Router);

  readonly vitalSigns = signal<VitalSign[]>([]);
  readonly loading = signal(false);
  readonly errorMessage = signal('');
  readonly hasSearched = signal(false);

  currentPage = 1;
  pageSize = 10;

  search(): void {
    this.loading.set(true);
    this.errorMessage.set('');
    this.hasSearched.set(true);

    this.vitalSignService.getVitalSigns().subscribe({
      next: vitalSigns => {
        this.vitalSigns.set(vitalSigns);
        this.currentPage = 1;
        this.loading.set(false);
      },
      error: error => {
        console.error(
          'Erro ao buscar sinais vitais:',
          error
        );
        this.vitalSigns.set([]);
        this.errorMessage.set(
          'Não foi possível buscar os sinais vitais.'
        );
        this.loading.set(false);
      }
    });
  }

  clearSearch(): void {
    this.vitalSigns.set([]);
    this.errorMessage.set('');
    this.loading.set(false);
    this.hasSearched.set(false);
    this.currentPage = 1;
  }

  get paginatedVitalSigns(): VitalSign[] {
    const start = (this.currentPage - 1) * this.pageSize;
    return this.vitalSigns().slice(
      start,
      start + this.pageSize
    );
  }

  get totalPages(): number {
    return Math.ceil(
      this.vitalSigns().length / this.pageSize
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

  formatBloodPressure(
    systolic?: number | null,
    diastolic?: number | null
  ): string {
    if (systolic == null && diastolic == null) {
      return '-';
    }

    return `${systolic ?? '-'} / ${diastolic ?? '-'}`;
  }
}
