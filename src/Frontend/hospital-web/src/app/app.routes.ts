import { Routes } from '@angular/router';

import { Shell } from './layout/shell/shell';

import {
  PatientList
} from './features/patients/patient-list/patient-list';

import {
  Patient360Component
} from './features/patients/patient-360/patient-360';

export const routes: Routes = [
  {
    path: '',
    component: Shell,
    children: [
      {
        path: '',
        redirectTo: 'patients',
        pathMatch: 'full'
      },
      {
        path: 'patients',
        component: PatientList
      },
      {
        path: 'patients/:id/360',
        component: Patient360Component
      }
    ]
  }
];