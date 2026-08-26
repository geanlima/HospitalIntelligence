import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Alert } from '../models/alert.model';

@Injectable({
  providedIn: 'root'
})
export class AlertService {
  private readonly http = inject(HttpClient);

  getAlerts(
    status?: string,
    severity?: string
  ): Observable<Alert[]> {
    let params = new HttpParams();

    if (status?.trim()) {
      params = params.set('status', status.trim());
    }

    if (severity?.trim()) {
      params = params.set('severity', severity.trim());
    }

    return this.http.get<Alert[]>('/alerts', { params });
  }
}
