import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { Identity } from '../auth/models/identity';
import { AuthService } from '../auth/services/auth.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss'],
})
export class NavbarComponent implements OnInit {
  isExpanded: boolean = false;
  constructor(private authService: AuthService) {}

  ngOnInit(): void {}

  isLoggedIn() {
    return this.authService.isLoggedIn;
  }

  collapse(): void {
    this.isExpanded = false;
  }

  toggle(): void {
    this.isExpanded = !this.isExpanded;
  }

  logout() {
    this.authService.logout();
  }
}
