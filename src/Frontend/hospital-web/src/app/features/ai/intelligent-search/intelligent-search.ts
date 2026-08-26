import { CommonModule } from '@angular/common';
import {
  Component,
  inject,
  OnInit,
  signal
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';

import {
  AiSource,
  AskAiResponse,
  IndexPatientClinicalRecordsResponse,
  SearchClinicalKnowledgeResponse
} from '../../../core/models/ai.model';
import { Patient } from '../../../core/models/patient.model';
import { AiService } from '../../../core/services/ai.service';
import { PatientService } from '../../../core/services/patient.service';

@Component({
  selector: 'app-intelligent-search',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterLink
  ],
  templateUrl: './intelligent-search.html',
  styleUrl: './intelligent-search.scss'
})
export class IntelligentSearchComponent implements OnInit {
  private readonly aiService = inject(AiService);
  private readonly patientService = inject(PatientService);
  private readonly route = inject(ActivatedRoute);

  readonly patients = signal<Patient[]>([]);
  readonly selectedPatient = signal<Patient | null>(null);
  readonly indexResult = signal<IndexPatientClinicalRecordsResponse | null>(null);
  readonly searchResult = signal<SearchClinicalKnowledgeResponse | null>(null);
  readonly askResult = signal<AskAiResponse | null>(null);

  readonly loadingPatients = signal(false);
  readonly indexing = signal(false);
  readonly searching = signal(false);
  readonly asking = signal(false);
  readonly errorMessage = signal('');
  readonly statusMessage = signal('');

  patientSearchName = '';
  selectedPatientId = '';
  searchQuery = '';
  askQuestion = '';

  ngOnInit(): void {
    const patientId =
      this.route.snapshot.queryParamMap.get('patientId');

    if (patientId) {
      this.selectedPatientId = patientId;
      this.loadPatientById(patientId);
    }
  }

  searchPatients(): void {
    this.loadingPatients.set(true);
    this.errorMessage.set('');

    this.patientService
      .getPatients(this.patientSearchName.trim())
      .subscribe({
        next: patients => {
          this.patients.set(patients);
          this.loadingPatients.set(false);

          if (
            this.selectedPatientId &&
            patients.some(p => p.id === this.selectedPatientId)
          ) {
            this.selectPatient(this.selectedPatientId);
          }
        },
        error: () => {
          this.patients.set([]);
          this.loadingPatients.set(false);
          this.errorMessage.set(
            'Não foi possível buscar pacientes.'
          );
        }
      });
  }

  selectPatient(patientId: string): void {
    this.selectedPatientId = patientId;

    const found = this.patients().find(p => p.id === patientId);

    if (found) {
      this.selectedPatient.set(found);
      return;
    }

    if (patientId) {
      this.loadPatientById(patientId);
    } else {
      this.selectedPatient.set(null);
    }
  }

  indexChart(): void {
    const patientId = this.selectedPatientId;

    if (!patientId) {
      this.errorMessage.set(
        'Selecione um paciente para indexar o prontuário.'
      );
      return;
    }

    this.indexing.set(true);
    this.errorMessage.set('');
    this.statusMessage.set('');
    this.indexResult.set(null);

    this.aiService.indexPatient(patientId).subscribe({
      next: result => {
        this.indexResult.set(result);
        this.statusMessage.set(
          `Prontuário indexado: ${result.indexedCount} registro(s).`
        );
        this.indexing.set(false);
      },
      error: error => {
        this.indexing.set(false);
        this.errorMessage.set(
          this.readApiError(
            error,
            'Não foi possível indexar o prontuário.'
          )
        );
      }
    });
  }

  runSemanticSearch(): void {
    const patientId = this.selectedPatientId;
    const query = this.searchQuery.trim();

    if (!patientId) {
      this.errorMessage.set(
        'Selecione um paciente para a busca semântica.'
      );
      return;
    }

    if (!query) {
      this.errorMessage.set(
        'Informe um termo para a busca semântica.'
      );
      return;
    }

    this.searching.set(true);
    this.errorMessage.set('');
    this.searchResult.set(null);

    this.aiService
      .search({
        query,
        patientId,
        topK: 5
      })
      .subscribe({
        next: result => {
          this.searchResult.set(result);
          this.searching.set(false);
        },
        error: error => {
          this.searching.set(false);
          this.errorMessage.set(
            this.readApiError(
              error,
              'Não foi possível executar a busca semântica.'
            )
          );
        }
      });
  }

  askQuestionAboutChart(): void {
    const patientId = this.selectedPatientId;
    const question = this.askQuestion.trim();

    if (!patientId) {
      this.errorMessage.set(
        'Selecione um paciente para perguntar à IA.'
      );
      return;
    }

    if (!question) {
      this.errorMessage.set(
        'Informe uma pergunta clínica.'
      );
      return;
    }

    this.asking.set(true);
    this.errorMessage.set('');
    this.askResult.set(null);

    this.aiService
      .ask({
        question,
        patientId,
        promptKey: 'clinical-chart-qa'
      })
      .subscribe({
        next: result => {
          this.askResult.set(result);
          this.asking.set(false);
        },
        error: error => {
          this.asking.set(false);
          this.errorMessage.set(
            this.readApiError(
              error,
              'Não foi possível obter resposta da IA.'
            )
          );
        }
      });
  }

  formatScore(score: number): string {
    return score.toFixed(3);
  }

  trackSource(_index: number, source: AiSource): string {
    return source.sourceId;
  }

  private loadPatientById(patientId: string): void {
    this.loadingPatients.set(true);

    this.patientService.getPatients().subscribe({
      next: patients => {
        this.patients.set(patients);
        this.loadingPatients.set(false);

        const found = patients.find(p => p.id === patientId);

        if (found) {
          this.selectedPatient.set(found);
          this.selectedPatientId = found.id;
        } else {
          this.errorMessage.set(
            'Paciente informado na URL não foi encontrado.'
          );
        }
      },
      error: () => {
        this.loadingPatients.set(false);
        this.errorMessage.set(
          'Não foi possível carregar pacientes.'
        );
      }
    });
  }

  private readApiError(
    error: unknown,
    fallback: string
  ): string {
    const description =
      (error as { error?: { description?: string } })
        ?.error?.description;

    return description?.trim() || fallback;
  }
}
