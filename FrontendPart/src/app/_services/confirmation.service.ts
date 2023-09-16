import { Injectable } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ConfirmationDialogComponent } from '../modals/confirmation-dialog/confirmation-dialog.component';
import { Observable, map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ConfirmationService {
  bsModalRef?: BsModalRef<ConfirmationDialogComponent>;

  constructor(private modalService: BsModalService) { }

  confirm(title = 'Confirmation',
    message = 'Are you sure that you really want to perform the intended action? This may cause loss of your unsaved data!',
    btnOkText = 'Okay',
    btnCancelText = 'Cancel') : Observable<boolean>
    {
      const config = {
        initialState: {
          title,
          message,
          btnOkText,
          btnCancelText
        }
      }
      this.bsModalRef = this.modalService.show(ConfirmationDialogComponent, config);
      return this.bsModalRef.onHidden.pipe(
        map(() =>{
          return this.bsModalRef!.content!.result;
        })
      );
  }
}
