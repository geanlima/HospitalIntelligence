import { CommonModule } from '@angular/common';
import {
  Component,
  inject,
  OnInit,
  signal
} from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { Patient360 } from '../../../core/models/patient-360.model';
import { PatientService } from '../../../core/services/patient.service';

type PatientTab =
  | 'overview'
  | 'admissions'
  | 'exams'
  | 'prescriptions'
  | 'vitalSigns'
  | 'clinicalNotes'
  | 'timeline'
  | 'alerts';

@Component({
  selector: 'app-patient-360',
  standalone: true,
  imports: [
    CommonModule
  ],
  templateUrl: './patient-360.html',
  styleUrl: './patient-360.scss'
})
export class Patient360Component implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly patientService = inject(PatientService);

  readonly patient = signal<Patient360 | null>(null);
  readonly loading = signal(false);
  readonly errorMessage = signal('');
  readonly activeTab = signal<PatientTab>('overview');

  ngOnInit(): void {
    const patientId =
      this.route.snapshot.paramMap.get('id');

    if (!patientId) {
      this.errorMessage.set(
        'Paciente não informado.'
      );

      return;
    }

    this.loadPatient(patientId);
  }

  selectTab(tab: PatientTab): void {
    this.activeTab.set(tab);
  }

  formatGender(gender: string): string {
    const map: Record<string, string> = {
      Male: 'Masculino',
      Female: 'Feminino',
      Other: 'Outro',
      Unknown: 'Não informado'
    };

    return map[gender] ?? gender;
  }

  formatStatus(status: string): string {
    const map: Record<string, string> = {
      Active: 'Ativo',
      Discharged: 'Alta',
      Cancelled: 'Cancelado',
      Requested: 'Solicitado',
      InProgress: 'Em andamento',
      Resulted: 'Concluído',
      Suspended: 'Suspenso',
      Completed: 'Concluído',
      Acknowledged: 'Reconhecido',
      Resolved: 'Resolvido'
    };

    return map[status] ?? status;
  }

  formatSeverity(severity: string): string {
    const map: Record<string, string> = {
      Low: 'Baixa',
      Medium: 'Média',
      High: 'Alta',
      Critical: 'Crítica'
    };

    return map[severity] ?? severity;
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'Active':
      case 'Resulted':
      case 'Completed':
      case 'Discharged':
        return 'status-success';

      case 'Requested':
      case 'InProgress':
      case 'Acknowledged':
      case 'Suspended':
        return 'status-warning';

      case 'Cancelled':
      case 'Resolved':
        return 'status-neutral';

      default:
        return 'status-neutral';
    }
  }

  getSeverityClass(severity: string): string {
    switch (severity) {
      case 'Critical':
        return 'severity-critical';

      case 'High':
        return 'severity-high';

      case 'Medium':
        return 'severity-medium';

      case 'Low':
        return 'severity-low';

      default:
        return 'severity-neutral';
    }
  }

  private loadPatient(
    patientId: string
  ): void {
    this.loading.set(true);
    this.errorMessage.set('');

    this.patientService
      .getPatient360(patientId)
      .subscribe({
        next: patient => {
          this.patient.set(patient);
          this.loading.set(false);
        },

        error: error => {
          console.error(
            'Erro ao carregar Patient 360:',
            error
          );

          this.patient.set(null);

          this.errorMessage.set(
            'Não foi possível carregar o Patient 360.'
          );

          this.loading.set(false);
        }
      });
  }
}