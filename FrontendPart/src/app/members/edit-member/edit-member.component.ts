import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-edit-member',
  templateUrl: './edit-member.component.html',
  styleUrls: ['./edit-member.component.css']
})
export class EditMemberComponent implements OnInit {
  @ViewChild('editForm') editForm: NgForm | undefined;
  @HostListener('window:beforeunload', ['$event']) unloadNotification($event:any) {
    if(this.editForm?.dirty) {
      $event.returnValue = true;
    }
  }
  member: Member | undefined;
  username: string = '';

  constructor(private memberService: MembersService, private route: ActivatedRoute, private toastr: ToastrService) {
    this.username = this.route.snapshot.params['username']
  }

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember() {
    if(this.username == '') return;
    this.memberService.getMember(this.username).subscribe({
      next: member => this.member = member
    })
  }

  updateProfileInfo() {
    this.memberService.updateMemberInfo(this.editForm?.value).subscribe({
      next: () => {
        this.toastr.success('Information has been updated successfully!');
        this.editForm?.reset(this.member);
      }
    })
  }

}
