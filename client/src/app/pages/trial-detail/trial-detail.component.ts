import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatDialogModule, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ClinicalTrial } from '../../models/clinical-trial.model';

@Component({
  selector: 'app-trial-detail',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatDividerModule,
    MatChipsModule,
    MatIconModule,
    MatListModule,
    MatDialogModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './trial-detail.component.html',
  styleUrls: ['./trial-detail.component.scss']
})
export class TrialDetailComponent {
  trial: ClinicalTrial;

  constructor(
    public dialogRef: MatDialogRef<TrialDetailComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { trial: ClinicalTrial }
  ) {
    this.trial = data.trial;
  }
}
