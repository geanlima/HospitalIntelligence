import { Routes } from '@angular/router';

import { authGuard, guestGuard } from './core/guards/auth.guard';
import { Shell } from './layout/shell/shell';

export const routes: Routes = [
  {
    path: 'login',
    canActivate: [guestGuard],
    loadComponent: () =>
      import('./features/auth/login/login').then(m => m.LoginComponent)
  },
  {
    path: '',
    component: Shell,
    canActivate: [authGuard],
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      },
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/dashboard/dashboard').then(
            m => m.DashboardComponent
          )
      },
      {
        path: 'patients',
        loadComponent: () =>
          import('./features/patients/patient-list/patient-list').then(
            m => m.PatientList
          )
      },
      {
        path: 'patients/:id/360',
        loadComponent: () =>
          import('./features/patients/patient-360/patient-360').then(
            m => m.Patient360Component
          )
      },
      {
        path: 'admissions',
        loadComponent: () =>
          import('./features/admissions/admission-list/admission-list').then(
            m => m.AdmissionListComponent
          )
      },
      {
        path: 'exams',
        loadComponent: () =>
          import('./features/exams/exam-list/exam-list').then(
            m => m.ExamListComponent
          )
      },
      {
        path: 'prescriptions',
        loadComponent: () =>
          import('./features/prescriptions/prescription-list/prescription-list').then(
            m => m.PrescriptionListComponent
          )
      },
      {
        path: 'vital-signs',
        loadComponent: () =>
          import('./features/vital-signs/vital-sign-list/vital-sign-list').then(
            m => m.VitalSignListComponent
          )
      },
      {
        path: 'clinical-notes',
        loadComponent: () =>
          import('./features/clinical-notes/clinical-note-list/clinical-note-list').then(
            m => m.ClinicalNoteListComponent
          )
      },
      {
        path: 'alerts',
        loadComponent: () =>
          import('./features/alerts/alert-list/alert-list').then(
            m => m.AlertListComponent
          )
      },
      {
        path: 'command-center',
        loadComponent: () =>
          import('./features/command-center/command-center').then(
            m => m.CommandCenterComponent
          )
      },
      {
        path: 'ai/search',
        loadComponent: () =>
          import('./features/ai/intelligent-search/intelligent-search').then(
            m => m.IntelligentSearchComponent
          )
      },
      {
        path: 'ai/audit',
        loadComponent: () =>
          import('./features/ai/chart-audit/chart-audit').then(
            m => m.ChartAuditComponent
          )
      },
      {
        path: 'ai/clinical-safety',
        loadComponent: () =>
          import('./features/ai/clinical-safety/clinical-safety').then(
            m => m.ClinicalSafetyComponent
          )
      },
      {
        path: 'reports',
        loadComponent: () =>
          import('./features/reports/reports').then(
            m => m.ReportsComponent
          )
      }
    ]
  }
];
