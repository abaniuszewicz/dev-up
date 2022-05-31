import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProfileDetailsComponent } from './profile-details/profile-details.component';
import { RouterModule, Routes } from '@angular/router';
import { AuthService } from '../auth/services/auth.service';

const routes: Routes = [{ path: '', component: ProfileDetailsComponent }];

@NgModule({
  declarations: [ProfileDetailsComponent],
  imports: [CommonModule, RouterModule.forChild(routes)],
  exports: [RouterModule],
  providers: [AuthService],
})
export class ProfileModule {}
