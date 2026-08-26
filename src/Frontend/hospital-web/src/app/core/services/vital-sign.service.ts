import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { VitalSign } from '../models/vital-sign.model';

@Injectable({
  providedIn: 'root'
})
export class VitalSignService {
  private readonly http = inject(HttpClient);

  getVitalSigns(): Observable<VitalSign[]> {
    return this.http.get<VitalSign[]>('/vital-signs');
  }
}
