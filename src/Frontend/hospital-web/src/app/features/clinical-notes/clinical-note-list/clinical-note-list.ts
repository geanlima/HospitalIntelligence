import { CommonModule } from '@angular/common';
import {
  Component,
  inject,
  signal
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { ClinicalNote } from '../../../core/models/clinical-note.model';
import { ClinicalNoteService } from '../../../core/services/clinical-note.service';

@Component({
  selector: 'app-clinical-note-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule
  ],
  templateUrl: './clinical-note-list.html',
  styleUrl: './clinical-note-list.scss'
})
export class ClinicalNoteListComponent {
  private readonly clinicalNoteService =
    inject(ClinicalNoteService);

  private readonly router = inject(Router);

  readonly notes = signal<ClinicalNote[]>([]);
  readonly loading = signal(false);
  readonly errorMessage = signal('');
  readonly hasSearched = signal(false);

  noteType = '';

  currentPage = 1;
  pageSize = 10;

  search(): void {
    this.loading.set(true);
    this.errorMessage.set('');
    this.hasSearched.set(true);

    this.clinicalNoteService
      .getClinicalNotes(this.noteType)
      .subscribe({
        next: notes => {
          this.notes.set(notes);
          this.currentPage = 1;
          this.loading.set(false);
        },
        error: error => {
          console.error(
            'Erro ao buscar notas clínicas:',
            error
          );
          this.notes.set([]);
          this.errorMessage.set(
            'Não foi possível buscar as notas clínicas.'
          );
          this.loading.set(false);
        }
      });
  }

  clearSearch(): void {
    this.noteType = '';
    this.notes.set([]);
    this.errorMessage.set('');
    this.loading.set(false);
    this.hasSearched.set(false);
    this.currentPage = 1;
  }

  get paginatedNotes(): ClinicalNote[] {
    const start = (this.currentPage - 1) * this.pageSize;
    return this.notes().slice(start, start + this.pageSize);
  }

  get totalPages(): number {
    return Math.ceil(this.notes().length / this.pageSize);
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

  formatNoteType(noteType: string): string {
    switch (noteType) {
      case 'Evolution':
        return 'Evolução';
      case 'Nursing':
        return 'Enfermagem';
      case 'Medical':
        return 'Médica';
      case 'Physiotherapy':
        return 'Fisioterapia';
      case 'Nutrition':
        return 'Nutrição';
      case 'Psychology':
        return 'Psicologia';
      case 'Other':
        return 'Outra';
      default:
        return noteType;
    }
  }
}
