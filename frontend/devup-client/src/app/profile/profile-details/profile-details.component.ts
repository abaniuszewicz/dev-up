import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { Identity } from 'src/app/auth/models/identity';
import { AuthService } from 'src/app/auth/services/auth.service';

@Component({
  selector: 'app-profile-details',
  templateUrl: './profile-details.component.html',
  styleUrls: ['./profile-details.component.scss'],
})
export class ProfileDetailsComponent implements OnInit {
  user$: Observable<Identity> = new Observable<Identity>();
  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    this.user$ = this.authService.userSubject$;
  }
}
