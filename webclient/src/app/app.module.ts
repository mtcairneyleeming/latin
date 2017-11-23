import {BrowserModule} from '@angular/platform-browser';
import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {AppComponent} from './app.component';
import {LandingComponent} from './pages/landing/landing.component';
import {SettingsComponent} from './pages/settings/settings.component';
import {ViewComponent} from './pages/lists/view/view.component';
import {EditComponent} from './pages/lists/edit/edit.component';
import {CreateComponent} from './pages/lists/create/create.component';
import {FindComponent} from './pages/lists/find/find.component';
import {LemmaComponent} from './pages/lemma/lemma.component';
import {LearnComponent} from './pages/learn/learn.component';
import {LemmasListComponent} from './components/lemmas-list/lemmas-list.component';
import {ListCardComponent} from './components/list-card/list-card.component';

import {AuthService} from './services/auth.service';
import {API} from './api/api';
import {ProfileComponent} from './components/profile/profile.component';
import {FormsModule} from '@angular/forms';
import {Ng2CompleterModule} from 'ng2-completer';

const appRoutes: Routes = [
  {path: '', component: LandingComponent},
  {path: 'learn/section/:sectionID', component: LearnComponent},
  {path: 'lists/find', component: FindComponent},
  {path: 'lists/create', component: CreateComponent},
  {path: 'lists/:listID', component: ViewComponent},
  {path: 'lists/:listID/edit', component: EditComponent},
  {path: 'lemma/:id', component: LemmaComponent},
  {path: 'profile', component: ProfileComponent},
  {path: '**', component: LandingComponent}
];

@NgModule({
  declarations: [
    AppComponent,
    LandingComponent,
    SettingsComponent,
    ViewComponent,
    EditComponent,
    CreateComponent,
    FindComponent,
    LemmaComponent,
    LearnComponent,
    LemmasListComponent,
    ListCardComponent,
    ProfileComponent,

  ],
  imports: [
    BrowserModule,
    RouterModule.forRoot(
      appRoutes,
      {enableTracing: false} // <-- debugging purposes only
    ),
    FormsModule,
    Ng2CompleterModule
  ],
  providers: [AuthService, API],
  bootstrap: [AppComponent]
})
export class AppModule {
}
