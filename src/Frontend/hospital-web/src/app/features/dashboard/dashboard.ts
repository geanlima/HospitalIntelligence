import { CommonModule } from '@angular/common';
import {
  Component,
  inject,
  OnInit,
  signal
} from '@angular/core';
import { Router } from '@angular/router';

import { DashboardSummary } from '../../core/models/dashboard-summary.model';
import { DashboardService } from '../../core/services/dashboard.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule
  ],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class DashboardComponent implements OnInit {
  private readonly router = inject(Router);
  private readonly dashboardService = inject(DashboardService);

  readonly summary = signal<DashboardSummary | null>(null);
  readonly loading = signal(false);
  readonly errorMessage = signal('');

  ngOnInit(): void {
    this.loadDashboard();
  }

  loadDashboard(): void {
    this.loading.set(true);
    this.errorMessage.set('');

    this.dashboardService.getSummary().subscribe({
      next: summary => {
        this.summary.set(summary);
        this.loading.set(false);
      },
      error: error => {
        console.error(
          'Erro ao carregar o dashboard:',
          error
        );

        this.errorMessage.set(
          'Não foi possível carregar os indicadores do dashboard.'
        );

        this.loading.set(false);
      }
    });
  }

  goToPatients(): void {
    this.router.navigate(['/patients']);
  }

  goToAdmissions(): void {
    this.router.navigate(['/admissions']);
  }

  goToExams(): void {
    this.router.navigate(['/exams']);
  }

  goToPrescriptions(): void {
    this.router.navigate(['/prescriptions']);
  }

  goToVitalSigns(): void {
    this.router.navigate(['/vital-signs']);
  }

  goToAlerts(): void {
    this.router.navigate(['/alerts']);
  }

  goToClinicalNotes(): void {
    this.router.navigate(['/clinical-notes']);
  }

  goToIntelligentSearch(): void {
    this.router.navigate(['/ai/search']);
  }

  goToChartAudit(): void {
    this.router.navigate(['/ai/audit']);
  }

  goToClinicalSafety(): void {
    this.router.navigate(['/ai/clinical-safety']);
  }
}