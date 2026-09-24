import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { SimulatorService } from '../services/simulator.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {

  loginForm = new FormGroup({
    username: new FormControl(''),
    password: new FormControl('')
  });

  constructor(private simulatorService: SimulatorService, private router: Router) {}

  sqlInjectionDetected = false;
  popupMessage = '';
  showPopup = false;

  onLogin() {
    const username = this.loginForm.value.username ?? '';
    const password = this.loginForm.value.password ?? '';

    this.simulatorService.login(username, password).subscribe({
      next: (response) => {
        console.log(response);
        this.router.navigate(['/home']);
      },
      error: (error) => {
        const wafResponse = JSON.parse(error.error);

        this.popupMessage = wafResponse.message;
        this.sqlInjectionDetected = wafResponse.sqlInjectionDetected;
        this.showPopup = true;
      }
    });
  }
}