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
  AuditPatientChartResponse,
  ChartAuditFinding
} from '../../../core/models/ai.model';
import { Patient } from '../../../core/models/patient.model';
import { AiService } from '../../../core/services/ai.service';
import { PatientService } from '../../../core/services/patient.service';

@Component({
  selector: 'app-chart-audit',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterLink
  ],
  templateUrl: './chart-audit.html',
  styleUrl: './chart-audit.scss'
})
export class ChartAuditComponent implements OnInit {
  private readonly aiService = inject(AiService);
  private readonly patientService = inject(PatientService);
  private readonly route = inject(ActivatedRoute);

  readonly patients = signal<Patient[]>([]);
  readonly selectedPatient = signal<Patient | null>(null);
  readonly auditResult = signal<AuditPatientChartResponse | null>(null);

  readonly loadingPatients = signal(false);
  readonly auditing = signal(false);
  readonly errorMessage = signal('');

  patientSearchName = '';
  selectedPatientId = '';

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

  runAudit(): void {
    const patientId = this.selectedPatientId;

    if (!patientId) {
      this.errorMessage.set(
        'Selecione um paciente para auditar.'
      );
      return;
    }

    this.auditing.set(true);
    this.errorMessage.set('');
    this.auditResult.set(null);

    this.aiService.auditPatient(patientId).subscribe({
      next: result => {
        this.auditResult.set(result);
        this.auditing.set(false);
      },
      error: error => {
        this.auditing.set(false);
        this.errorMessage.set(
          this.readApiError(
            error,
            'Não foi possível auditar o prontuário.'
          )
        );
      }
    });
  }

  categoryLabel(category: string): string {
    const map: Record<string, string> = {
      MissingDocumentation: 'Documentação ausente',
      Divergence: 'Divergência',
      FinancialGlosaRisk: 'Risco de glosa'
    };

    return map[category] ?? category;
  }

  severityClass(severity: string): string {
    return `severity-${severity.toLowerCase()}`;
  }

  riskClass(risk: string): string {
    return `risk-${risk.toLowerCase()}`;
  }

  trackFinding(_index: number, finding: ChartAuditFinding): string {
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
