import {Component, OnInit} from '@angular/core';
import {AuthService} from '../../services/auth.service';
import {API} from "../../api/api";
import {List} from "../../models/List";

@Component({
  selector: 'app-landing',
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.css']
})
export class LandingComponent implements OnInit {
  lists: List[];

  constructor(private auth: AuthService,
              private http: API) {
  }

  ngOnInit() {
    this.http.getMyLists().then(lists => this.lists = lists);
  }

}
