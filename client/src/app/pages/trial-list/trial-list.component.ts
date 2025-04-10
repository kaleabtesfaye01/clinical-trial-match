import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { ClinicalTrial } from '../../models/clinical-trial.model';
import { TrialDetailComponent } from '../trial-detail/trial-detail.component';

interface Location {
  status?: string;
  city?: string;
  state?: string;
  country?: string;
}

@Component({
  selector: 'app-trial-list',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    MatDialogModule,
    TrialDetailComponent
  ],
  templateUrl: './trial-list.component.html',
  styleUrls: ['./trial-list.component.scss']
})
export class TrialListComponent {
  @Input() trials: ClinicalTrial[] = [];
  @Input() loading = false;

  constructor(private dialog: MatDialog) {}

  openTrialDetails(trial: ClinicalTrial): void {
    this.dialog.open(TrialDetailComponent, {
      data: { trial },
      width: '100%',
      maxWidth: '1200px',
      maxHeight: '90vh',
      panelClass: 'trial-detail-dialog'
    });
  }

  trackByNctId(_: number, trial: ClinicalTrial): string {
    return trial.nctId;
  }

  trackByCondition(_: number, condition: string): string {
    return condition;
  }

  trackByLocation(_: number, location: Location): string {
    return `${location.city || ''}-${location.state || ''}-${location.country || ''}`;
  }
}
