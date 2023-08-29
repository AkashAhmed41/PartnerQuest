import { Component, Input, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
  @Input() member: Member | undefined;

  constructor (private memberService: MembersService, private toastr: ToastrService) {

  }

  ngOnInit(): void {
    
  }

  addToFavourite(member: Member) {
    this.memberService.addToFavourite(member.userName).subscribe({
      next: () => this.toastr.success(member.nickname + ` has been successfully added to your Favourite!`)
    });
  }

}
