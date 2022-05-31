import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { RegisterRequest } from '../models/register-request';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
})
export class RegisterComponent implements OnInit {
  registerForm = new FormGroup({
    username: new FormControl('', [Validators.required]),
    password: new FormControl('', [Validators.required]),
  });

  constructor(private authService: AuthService) {}

  ngOnInit(): void {}

  onSubmit(): void {
    const registerRequest: RegisterRequest = {
      ...{
        device: {
          id: 'b691b7a8-b251-4b11-8034-f3a0a154dffe',
          name: 'iPhone (John)',
        },
      },
      ...this.registerForm.value,
    };

    this.authService.register(registerRequest);
  }
}
