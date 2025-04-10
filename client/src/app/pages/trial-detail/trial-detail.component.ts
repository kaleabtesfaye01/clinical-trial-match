import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { catchError, Observable, of } from 'rxjs';
import { ClinicalTrial } from '../../models/clinical-trial.model';
import { TrialService } from '../../core/services/trial.service';
import { NavMenuComponent } from '../../shared/components/nav-menu/nav-menu.component';
import { MarkdownModule, MarkdownService } from 'ngx-markdown';

@Component({
  selector: 'app-trial-detail',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatDividerModule,
    MatProgressSpinnerModule,
    NavMenuComponent,
    MarkdownModule,
  ],
  providers: [MarkdownService],
  templateUrl: './trial-detail.component.html',
  styleUrls: ['./trial-detail.component.scss'],
})
export class TrialDetailComponent implements OnInit {
  trial$?: Observable<ClinicalTrial>;
  loading = true;
  error = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private trialService: TrialService
  ) {}

  ngOnInit(): void {
    const nctId = this.route.snapshot.paramMap.get('id');
    if (!nctId) {
      this.router.navigate(['/trial-list']);
      return;
    }

    this.trial$ = this.trialService.getTrialById(nctId).pipe(
      catchError((err) => {
        console.error('Error loading trial:', err);
        this.error =
          err.message || 'Error loading trial details. Please try again later.';
        this.loading = false;
        return of(null as any);
      })
    );
  }

  goBack(): void {
    this.router.navigate(['/trial-list']);
  }

  formatDate(date: string): string {
    if (!date) return 'Not specified';
    return new Date(date).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
    });
  }

  showFullCriteria = false;

  toggleCriteria(): void {
    this.showFullCriteria = !this.showFullCriteria;
  }
}
