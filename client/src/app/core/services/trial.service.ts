import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ClinicalTrial } from '../../models/clinical-trial.model';
import { PatientData } from '../../models/patient-data.model';

@Injectable({
  providedIn: 'root',
})
export class TrialService {
  private baseUrl = 'http://localhost:5230/api/trials';

  constructor(private http: HttpClient) {}

  matchTrialsWithAi(input: PatientData): Observable<ClinicalTrial[]> {
    return this.http.post<ClinicalTrial[]>(`${this.baseUrl}/match`, input);
  }
}
