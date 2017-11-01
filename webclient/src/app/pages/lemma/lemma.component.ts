import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, ParamMap, Router} from '@angular/router';
import {Observable} from 'rxjs/Observable';
import 'rxjs/add/operator/switchMap';
import {API} from '../../api/api';
import * as md from '../../models/models';


@Component({
  selector: 'app-lemma',
  templateUrl: './lemma.component.html',
  styleUrls: ['./lemma.component.css']
})
export class LemmaComponent implements OnInit {
  lemma$: Observable<md.Lemmas>;

  constructor(public http: API,
              private route: ActivatedRoute,
              private router: Router) {
  }

  async ngOnInit() {
    this.lemma$ = this.route.paramMap
      .switchMap((params: ParamMap) =>
        this.http.getLemmaWithData(parseInt(params.get('id'), 10)));
  }

}