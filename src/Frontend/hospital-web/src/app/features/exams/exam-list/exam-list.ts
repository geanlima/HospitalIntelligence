import { CommonModule } from '@angular/common';
import {
  Component,
  inject,
  signal
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { Exam } from '../../../core/models/exam.model';
import { ExamService } from '../../../core/services/exam.service';

@Component({
  selector: 'app-exam-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule
  ],
  templateUrl: './exam-list.html',
  styleUrl: './exam-list.scss'
})
export class ExamListComponent {
  private readonly examService = inject(ExamService);
  private readonly router = inject(Router);

  readonly exams = signal<Exam[]>([]);
  readonly loading = signal(false);
  readonly errorMessage = signal('');
  readonly hasSearched = signal(false);

  status = '';
  name = '';

  currentPage = 1;
  pageSize = 10;

  search(): void {
    this.loading.set(true);
    this.errorMessage.set('');
    this.hasSearched.set(true);

    this.examService
      .getExams(this.status, this.name)
      .subscribe({
        next: exams => {
          this.exams.set(exams);
          this.currentPage = 1;
          this.loading.set(false);
        },
        error: error => {
          console.error('Erro ao buscar exames:', error);
          this.exams.set([]);
          this.errorMessage.set(
            'Não foi possível buscar os exames.'
          );
          this.loading.set(false);
        }
      });
  }

  clearSearch(): void {
    this.status = '';
    this.name = '';
    this.exams.set([]);
    this.errorMessage.set('');
    this.loading.set(false);
    this.hasSearched.set(false);
    this.currentPage = 1;
  }

  get paginatedExams(): Exam[] {
    const start = (this.currentPage - 1) * this.pageSize;
    return this.exams().slice(start, start + this.pageSize);
  }

  get totalPages(): number {
    return Math.ceil(this.exams().length / this.pageSize);
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
      case 'Requested':
        return 'Solicitado';
      case 'InProgress':
        return 'Em andamento';
      case 'Resulted':
        return 'Com resultado';
      case 'Cancelled':
        return 'Cancelado';
      default:
        return status;
    }
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'Requested':
        return 'status-requested';
      case 'InProgress':
        return 'status-inprogress';
      case 'Resulted':
        return 'status-resulted';
      case 'Cancelled':
        return 'status-cancelled';
      default:
        return 'status-neutral';
    }
  }
}
