import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators,
} from '@angular/forms';
import { PatientData } from '../../models/patient-data.model';
import { TrialService } from '../../core/services/trial.service';

@Component({
  selector: 'app-patient-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule], // Import ReactiveFormsModule here
  templateUrl: './patient-form.component.html',
  styleUrls: ['./patient-form.component.scss'],
})
export class PatientFormComponent implements OnInit {
  // The Reactive Form group
  patientForm!: FormGroup;

  submittedData: PatientData | null = null;
  matchingTrials: any[] = [];
  errorMessage: string = '';

  constructor(private fb: FormBuilder, private trialService: TrialService) {}

  ngOnInit(): void {
    this.initForm();
  }

  // Initialize the form with default values and validators
  initForm(): void {
    this.patientForm = this.fb.group({
      age: [null, [Validators.min(0), Validators.max(120)]],
      gender: [''],
      condition: [''],
      location: [''],
      notes: ['', [Validators.required]],
    });
  }

  // Convenience getter for easy access to form fields
  get f() {
    return this.patientForm.controls;
  }

  onSubmit(): void {
    if (this.patientForm.valid) {
      const input: PatientData = {
        age: this.f['age'].value,
        gender: this.f['gender'].value,
        condition: this.f['condition'].value,
        location: this.f['location'].value,
        notes: this.f['notes'].value,
      };

      this.trialService.matchTrialsWithAi(input).subscribe({
        next: (matchedTrials) => {
          this.matchingTrials = matchedTrials;
          this.submittedData = input; // for preview
        },
        error: (err) => {
          console.error('Error matching with AI', err);
          this.errorMessage = 'An error occurred while matching.';
        },
      });
    } else {
      this.patientForm.markAllAsTouched();
    }
  }
}
