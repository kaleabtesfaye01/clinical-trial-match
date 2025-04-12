import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, map } from 'rxjs';
import { ClinicalTrial } from '../../models/clinical-trial.model';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class TrialService {
  private apiUrl = `${environment.apiUrl}/trials`;
  private matchedTrialsSubject = new BehaviorSubject<ClinicalTrial[]>([]);
  matchedTrials$ = this.matchedTrialsSubject.asObservable();

  constructor(private http: HttpClient) {}

  matchTrialsWithAi(input: any): Observable<ClinicalTrial[]> {
    return this.http.post<ClinicalTrial[]>(`${this.apiUrl}/match`, input).pipe(
      map((trials) => {
        this.setMatchedTrials(trials);
        return trials;
      })
    );
  }

  setMatchedTrials(trials: ClinicalTrial[]): void {
    this.matchedTrialsSubject.next(trials);
  }

  getTrialById(nctId: string): Observable<ClinicalTrial> {
    return this.matchedTrials$.pipe(
      map((trials) => {
        const trial = trials.find((t) => t.nctId === nctId);
        if (!trial) {
          throw new Error('Trial not found');
        }
        return trial;
      })
    );
  }
}
