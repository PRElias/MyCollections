// Copyright 2016 Google Inc.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.


(function() {
  'use strict';

  var app = {
    isLoading: true,
    visibleCards: {},
    selectedGames: [],
    spinner: document.querySelector('.loader'),
    cardTemplate: document.querySelector('.cardTemplate'),
    container: document.querySelector('.main')
  };


  document.getElementById('butRefresh').addEventListener('click', function() {
      app.getGames();
  });

  app.getGames = function() {
    var url = 'http://localhost:8091/Api/Games';
    var request = new XMLHttpRequest();
    var items = [];
    request.onreadystatechange = function() {
      if (request.readyState === XMLHttpRequest.DONE) {
        if (request.status === 200) {

          for(var game in request.response){
              console.log(game);
              items.push(
                "<li " +  game.name + "/li></div>"
              );
          }

          $("<ul/>", {
            "class": "list-group",
            html: items.join("")
          }).appendTo("#div_game");

        $("#div_game").wrap("<div class='card cardTemplate' id='res_wrap'></div>");

        }
    };
    request.open('GET', url);
    request.send();
  };

  // TODO add saveSelectedGames function here
  // Save list of cities to localStorage.
  app.saveSelectedGames = function() {
    var selectedGames = JSON.stringify(app.selectedGames);
    localStorage.selectedGames = selectedGames;
    };
  };

  // TODO add service worker code here
  if ('serviceWorker' in navigator) {
    navigator.serviceWorker
             .register('./service-worker.js')
             .then(function() { console.log('Service Worker Registered'); });
  }
});
