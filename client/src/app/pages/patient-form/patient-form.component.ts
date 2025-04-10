import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators,
} from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { PatientData } from '../../models/patient-data.model';
import { TrialService } from '../../core/services/trial.service';
import { ClinicalTrial } from '../../models/clinical-trial.model';
import { TrialListComponent } from '../trial-list/trial-list.component';

@Component({
  selector: 'app-patient-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatCardModule,
    TrialListComponent
  ],
  templateUrl: './patient-form.component.html',
  styleUrls: ['./patient-form.component.scss'],
})
export class PatientFormComponent implements OnInit {
  patientForm!: FormGroup;
  submittedData: PatientData | null = null;
  matchingTrials: ClinicalTrial[] = [];
  loading = false;
  errorMessage: string = '';

  constructor(private fb: FormBuilder, private trialService: TrialService) {}

  ngOnInit(): void {
    this.initForm();
  }

  initForm(): void {
    this.patientForm = this.fb.group({
      notes: ['', [Validators.required]],
      age: [null, [Validators.min(0), Validators.max(120)]],
      sex: [''],
      condition: [''],
      city: [''],
      state: [''],
      country: ['']
    });
  }

  get f() {
    return this.patientForm.controls;
  }

  onSubmit(): void {
    if (this.patientForm.valid) {
      this.loading = true;
      this.errorMessage = '';
      
      const input: PatientData = {
        notes: this.f['notes'].value,
        age: this.f['age'].value,
        sex: this.f['sex'].value,
        condition: this.f['condition'].value,
        city: this.f['city'].value,
        state: this.f['state'].value,
        country: this.f['country'].value
      };

      this.trialService.matchTrialsWithAi(input).subscribe({
        next: (matchedTrials) => {
          this.matchingTrials = matchedTrials;
          this.submittedData = input;
          this.loading = false;
        },
        error: (err) => {
          console.error('Error matching with AI', err);
          this.errorMessage = 'An error occurred while matching trials.';
          this.loading = false;
        }
      });
    } else {
      this.patientForm.markAllAsTouched();
    }
  }
}
