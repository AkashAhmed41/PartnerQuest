import { Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-confirmation-dialog',
  templateUrl: './confirmation-dialog.component.html',
  styleUrls: ['./confirmation-dialog.component.css']
})
export class ConfirmationDialogComponent implements OnInit {
  title = '';
  message = '';
  btnOkText = '';
  btnCancelText = '';
  result = false;

  constructor(public bsModalRef: BsModalRef) {}

  ngOnInit(): void {
    
  }

  confirm() {
    this.result = true;
    this.bsModalRef.hide();
  }

  cancel() {
    this.bsModalRef.hide();
  }

}
