import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Prescription } from '../models/prescription.model';

@Injectable({
  providedIn: 'root'
})
export class PrescriptionService {
  private readonly http = inject(HttpClient);

  getPrescriptions(
    status?: string
  ): Observable<Prescription[]> {
    let params = new HttpParams();

    if (status?.trim()) {
      params = params.set('status', status.trim());
    }

    return this.http.get<Prescription[]>(
      '/prescriptions',
      { params }
    );
  }
}
