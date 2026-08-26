import { CommonModule } from '@angular/common';
import {
  Component,
  inject,
  OnInit,
  signal
} from '@angular/core';
import { RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';

interface CommandCenterSummary {
  operational: {
    totalPatients: number;
    activeAdmissions: number;
    pendingExams: number;
    criticalAlerts: number;
    occupancyPercentage: number;
    occupiedBeds: number;
    availableBeds: number;
    totalBeds: number;
  };
  predictedDischargesToday: number;
  elevatedDeteriorationCount: number;
  highNoShowRiskCount: number;
  topInsights: Array<{
    patientId: string;
    dischargeLabel: string;
    dischargeScore: number;
    deteriorationLabel: string;
    deteriorationScore: number;
  }>;
  generatedAtUtc: string;
}

@Component({
  selector: 'app-command-center',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './command-center.html',
  styleUrl: './command-center.scss'
})
export class CommandCenterComponent implements OnInit {
  private readonly http = inject(HttpClient);

  readonly summary = signal<CommandCenterSummary | null>(null);
  readonly loading = signal(false);
  readonly errorMessage = signal('');

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.errorMessage.set('');

    this.http.get<CommandCenterSummary>('/command-center/summary').subscribe({
      next: summary => {
        this.summary.set(summary);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.errorMessage.set(
          'Não foi possível carregar o Command Center.'
        );
      }
    });
  }
}
