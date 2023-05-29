import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { EditMemberComponent } from '../members/edit-member/edit-member.component';

@Injectable({
  providedIn: 'root'
})
export class NavigationPreventingGuard implements CanDeactivate<EditMemberComponent> {
  canDeactivate(component: EditMemberComponent): boolean {
    if(component.editForm?.dirty) {
      return confirm('Any unsaved changes will be lost! Do you really want to leave the page?');
    }
    return true;
  }
  
}
