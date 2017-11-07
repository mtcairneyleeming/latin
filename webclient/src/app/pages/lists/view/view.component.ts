import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, ParamMap} from '@angular/router';
import {Observable} from 'rxjs/Observable';
import 'rxjs/add/observable/fromPromise';
import 'rxjs/add/operator/switchMap';
import {API} from '../../../api/api';
import * as md from '../../../models/models';


@Component({
  selector: 'app-view',
  templateUrl: './view.component.html',
  styleUrls: ['./view.component.css']
})
export class ViewComponent implements OnInit {
  list: Observable<md.List>;
  sections: Observable<md.Section[]>;

  constructor(public http: API,
              private route: ActivatedRoute) {
  }

  ngOnInit() {
    let id = 0;
    this.list = this.route.paramMap
      .switchMap((params: ParamMap) => {
        id = parseInt(params.get('listID'), 10);

        this.sections = Observable.fromPromise(this.http.getSectionsInList(id));
        return this.http.getList(id);
      });
  }
}


