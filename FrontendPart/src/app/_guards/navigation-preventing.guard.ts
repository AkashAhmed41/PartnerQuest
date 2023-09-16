import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { EditMemberComponent } from '../members/edit-member/edit-member.component';
import { ConfirmationService } from '../_services/confirmation.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class NavigationPreventingGuard implements CanDeactivate<EditMemberComponent> {

  constructor(private confirmationService: ConfirmationService) {}

  canDeactivate(component: EditMemberComponent): Observable<boolean> | boolean {
    if(component.editForm?.dirty) {
      return this.confirmationService.confirm();
    }
    return true;
  }
  
}
