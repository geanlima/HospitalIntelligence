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
  AssessClinicalSafetyResponse,
  ClinicalSafetyFinding,
  StructureVoiceNoteResponse
} from '../../../core/models/ai.model';
import { Patient } from '../../../core/models/patient.model';
import { AiService } from '../../../core/services/ai.service';
import { PatientService } from '../../../core/services/patient.service';

@Component({
  selector: 'app-clinical-safety',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterLink
  ],
  templateUrl: './clinical-safety.html',
  styleUrl: './clinical-safety.scss'
})
export class ClinicalSafetyComponent implements OnInit {
  private readonly aiService = inject(AiService);
  private readonly patientService = inject(PatientService);
  private readonly route = inject(ActivatedRoute);

  readonly patients = signal<Patient[]>([]);
  readonly selectedPatient = signal<Patient | null>(null);
  readonly assessment = signal<AssessClinicalSafetyResponse | null>(null);
  readonly voiceDraft = signal<StructureVoiceNoteResponse | null>(null);

  readonly loadingPatients = signal(false);
  readonly assessing = signal(false);
  readonly structuring = signal(false);
  readonly errorMessage = signal('');

  patientSearchName = '';
  selectedPatientId = '';
  voiceTranscript = '';
  voiceNoteType = 'Evolution';

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
          this.errorMessage.set('Não foi possível buscar pacientes.');
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

  runAssessment(): void {
    if (!this.selectedPatientId) {
      this.errorMessage.set('Selecione um paciente.');
      return;
    }

    this.assessing.set(true);
    this.errorMessage.set('');
    this.assessment.set(null);

    this.aiService.assessClinicalSafety(this.selectedPatientId).subscribe({
      next: result => {
        this.assessment.set(result);
        this.assessing.set(false);
      },
      error: error => {
        this.assessing.set(false);
        this.errorMessage.set(
          this.readApiError(error, 'Não foi possível avaliar a segurança clínica.')
        );
      }
    });
  }

  structureVoiceNote(): void {
    if (!this.voiceTranscript.trim()) {
      this.errorMessage.set('Informe a transcrição da nota.');
      return;
    }

    this.structuring.set(true);
    this.errorMessage.set('');
    this.voiceDraft.set(null);

    this.aiService
      .structureVoiceNote({
        transcript: this.voiceTranscript.trim(),
        patientId: this.selectedPatientId || null,
        noteType: this.voiceNoteType
      })
      .subscribe({
        next: result => {
          this.voiceDraft.set(result);
          this.structuring.set(false);
        },
        error: error => {
          this.structuring.set(false);
          this.errorMessage.set(
            this.readApiError(error, 'Não foi possível estruturar a nota.')
          );
        }
      });
  }

  categoryLabel(category: string): string {
    const map: Record<string, string> = {
      DischargeSafety: 'Alta segura',
      Deterioration: 'Deterioração',
      MedicationReconciliation: 'Reconciliação medicamentosa',
      TriageAssist: 'Copiloto de triagem'
    };

    return map[category] ?? category;
  }

  severityClass(severity: string): string {
    return `severity-${severity.toLowerCase()}`;
  }

  riskClass(risk: string): string {
    return `risk-${risk.toLowerCase()}`;
  }

  trackFinding(_index: number, finding: ClinicalSafetyFinding): string {
    return finding.code;
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
        this.errorMessage.set('Não foi possível carregar pacientes.');
      }
    });
  }

  private readApiError(error: unknown, fallback: string): string {
    const description =
      (error as { error?: { description?: string } })
        ?.error?.description;

    return description?.trim() || fallback;
  }
}
