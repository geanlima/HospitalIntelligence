export interface DashboardActivity {
  id: string;
  occurredAtUtc: string;
  type: string;
  title: string;
  description: string;
}

export interface DashboardSummary {
  totalPatients: number;
  activeAdmissions: number;
  pendingExams: number;
  criticalAlerts: number;

  totalBeds: number;
  occupiedBeds: number;
  availableBeds: number;
  occupancyPercentage: number;

  recentActivities: DashboardActivity[];
}