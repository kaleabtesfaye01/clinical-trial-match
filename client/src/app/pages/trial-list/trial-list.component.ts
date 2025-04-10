import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { ClinicalTrial } from '../../models/clinical-trial.model';
import { TrialService } from '../../core/services/trial.service';
import { NavMenuComponent } from '../../shared/components/nav-menu/nav-menu.component';

@Component({
  selector: 'app-trial-list',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatChipsModule,
    NavMenuComponent,
  ],
  templateUrl: './trial-list.component.html',
  styleUrls: ['./trial-list.component.scss'],
})
export class TrialListComponent implements OnInit, OnDestroy {
  trials: ClinicalTrial[] = [];
  loading = true;
  private subscription?: Subscription;

  constructor(private trialService: TrialService, private router: Router) {}

  ngOnInit(): void {
    this.subscription = this.trialService.matchedTrials$.subscribe((trials) => {
      this.trials = trials;
      this.loading = false;
    });
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
  }

  startNewSearch(): void {
    this.router.navigate(['/patient-form']);
  }

  viewTrialDetails(trial: ClinicalTrial): void {
    this.router.navigate(['/trial', trial.nctId]);
  }

  formatLocation(location: {
    city?: string;
    state?: string;
    country?: string;
  }): string {
    const parts = [];
    if (location.city) parts.push(location.city);
    if (location.state) parts.push(location.state);
    if (location.country) parts.push(location.country);
    return parts.join(', ') || 'Location not specified';
  }
}
