import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, ParamMap, Router} from '@angular/router';
import {Observable} from 'rxjs/Observable';
import {CompleterItem, CompleterService, RemoteData} from 'ng2-completer';

import 'rxjs/add/observable/fromPromise';
import 'rxjs/add/operator/switchMap';
import {API} from '../../../api/api';
import * as md from '../../../models/models';
import {ReplaySubject} from 'rxjs/ReplaySubject';


@Component({
  selector: 'app-edit',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.css']
})
export class EditComponent implements OnInit {
  list: Observable<md.Lists>;
  sections: ReplaySubject<md.Sections[]>;
  lemmaText: string;
  public lemmaDataServices: RemoteData[][];

  constructor(public http: API,
              private route: ActivatedRoute,
              private router: Router,
              private completerService: CompleterService) {
    this.lemmaDataServices = [];
  }

  ngOnInit() {
    let id = 0;
    this.list = this.route.paramMap
      .switchMap((params: ParamMap) => {
        id = parseInt(params.get('listID'), 10);

        this.sections = new ReplaySubject(1);
        const sectionObservables = Observable.fromPromise(this.http.getSectionsInList(id));
        sectionObservables.subscribe((sections: md.Sections[]) => {

          for (let s = 0; s < sections.length; s++) {
            this.lemmaDataServices.push([]);
            console.log(sections[s]);
            console.log(sections[s].sectionWords);
            for (let l = 0; l < sections[s].sectionWords.length; l++) {

              this.lemmaDataServices[s][l] = this.completerService.remote(null, 'lemmaText,lemmaId', 'lemmaText');
              this.lemmaDataServices[s][l].urlFormater(term => this.http.BaseUrl + `/lemmas/search?query=${term}`);

            }
          }
          console.log(this.lemmaDataServices);
          this.sections.next(sections);
        });
        return this.http.getList(id);
      });
  }

  selectItem(sectionId: number, lemmaId: number, data: CompleterItem) {
    const newLemma: md.Lemmas = data.originalObject;
    this.http.modifySectionLemmas(sectionId, [lemmaId], [newLemma.lemmaId]);
    this.sections.subscribe(
      (x) => {
        const section = x.find((s) => s.sectionId === sectionId);
        for (let i = 0; i < section.sectionWords.length; i++) {
          if (section.sectionWords[i].lemmaId = lemmaId) {
            section.sectionWords[i] = newLemma;
          }
        }
      });
  }
}
