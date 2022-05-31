import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { LoginRequest } from '../models/login-request';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit {
  loginForm = new FormGroup({
    username: new FormControl('', [Validators.required]),
    password: new FormControl('', [Validators.required]),
  });

  constructor(private authService: AuthService) {}

  ngOnInit(): void {}

  onSubmit(): void {
    const loginRequest: LoginRequest = {
      ...{
        device: {
          id: 'b691b7a8-b251-4b11-8034-f3a0a154dffe',
          name: 'iPhone (John)',
        },
      },
      ...this.loginForm.value,
    };

    this.authService.login(loginRequest);
  }
}
