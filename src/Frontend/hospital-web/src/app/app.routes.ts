import { Routes } from '@angular/router';

import { DashboardComponent } from './features/dashboard/dashboard';
import { Patient360Component } from './features/patients/patient-360/patient-360';
import { PatientList } from './features/patients/patient-list/patient-list';
import { Shell } from './layout/shell/shell';
import { AdmissionListComponent } from './features/admissions/admission-list/admission-list';

export const routes: Routes = [
  {
    path: '',
    component: Shell,
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      },
      {
        path: 'dashboard',
        component: DashboardComponent
      },
      {
        path: 'patients',
        component: PatientList
      },
      {
        path: 'patients/:id/360',
        component: Patient360Component
      },
      {
        path: 'admissions',
        component: AdmissionListComponent
      }
    ]
  }
];