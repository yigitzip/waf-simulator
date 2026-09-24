import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { SimulatorService } from '../services/simulator.service';

@Component({
  selector: 'app-home',
  imports: [ReactiveFormsModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent {

  commentForm = new FormGroup({
  comment: new FormControl('')
  });

  commentResponse = '';

  searchForm = new FormGroup({
  query: new FormControl('')
  });

  searchResponse = '';

  fileForm = new FormGroup({
  path: new FormControl('')
  });

  fileResponse = '';
  
  commandForm = new FormGroup({
  command: new FormControl('')
  });

  commandResponse = '';


  constructor(private simulatorService: SimulatorService) {}

  sqlInjectionDetected = false;
  popupMessage = '';
  showPopup = false;

onComment() {
  const comment = this.commentForm.value.comment ?? '';

  this.simulatorService.comment(comment).subscribe({
    next: (response) => {
      this.commentResponse = response;
    },
      error: (error) => {
        this.errorAndStuff(error);
      }
  });
}

    onSearch() {
    const query = this.searchForm.value.query ?? '';

    this.simulatorService.search(query).subscribe({
      next: (response) => {
        this.searchResponse = response;
      },
      error: (error) => {
        this.errorAndStuff(error);
      }
    });
  }

    onFileRequest() {
    const path = this.fileForm.value.path ?? '';

    this.simulatorService.testFilePath(path).subscribe({
      next: (response) => {
        this.fileResponse = response;
      },
      error: (error) => {
        this.errorAndStuff(error);
      }
    });
  }

    onCommand() {
    const command = this.commandForm.value.command ?? '';

    this.simulatorService.testCommand(command).subscribe({
      next: (response) => {
        this.commandResponse = response;
      },
      error: (error) => {
        this.errorAndStuff(error);
      }
    });
  }

  errorAndStuff(error : any) {

    const wafResponse = JSON.parse(error.error);

    this.popupMessage = wafResponse.message;
    this.sqlInjectionDetected = wafResponse.sqlInjectionDetected;
    this.showPopup = true;
  }
}