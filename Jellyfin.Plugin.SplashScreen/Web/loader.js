(function () {
  "use strict";

  var startTime = Date.now();

  fetch("/SplashScreen/Config")
    .then(function (response) {
      return response.json();
    })
    .then(function (config) {
      if (!config || !config.enabled) {
        return;
      }
      init(config);
    })
    .catch(function () {
      /* If config can't be fetched, don't block page load with a broken splash. */
    });

  function init(config) {
    var style = document.createElement("style");
    style.textContent =
      ".bar-loading {" +
      "  z-index: 2147483647;" +
      "  position: fixed;" +
      "  top: 0;" +
      "  left: 0;" +
      "  width: 100vw;" +
      "  height: 100vh;" +
      "  background-color: " + config.backgroundColor + ";" +
      "  display: flex;" +
      "  align-items: center;" +
      "  justify-content: center;" +
      "  opacity: 1;" +
      "  transition: opacity 0.3s ease-in-out;" +
      "  overflow: hidden;" +
      "  will-change: opacity;" +
      "}" +
      ".bar-loading.hide { opacity: 0; }" +
      ".jf-loader-content {" +
      "  position: relative;" +
      "  display: flex;" +
      "  flex-direction: column;" +
      "  align-items: center;" +
      "  gap: 20px;" +
      "  width: 250px;" +
      "  height: auto;" +
      "}" +
      ".jf-loader-content h1 {" +
      "  width: 250px;" +
      "  margin: 0 auto;" +
      "  height: 250px;" +
      "  display: flex;" +
      "  justify-content: center;" +
      "  align-items: center;" +
      "}" +
      ".jf-splash-logo {" +
      "  width: 250px;" +
      "  max-height: 250px;" +
      "  height: 100%;" +
      "  display: block;" +
      "  background-repeat: no-repeat;" +
      "  background-position: center;" +
      "  background-size: contain;" +
      "  opacity: 0;" +
      "  transition: opacity 0.5s ease-in-out;" +
      "}" +
      ".jf-loader-message {" +
      "  color: " + config.textColor + ";" +
      "  font-size: 1rem;" +
      "  text-align: center;" +
      "}" +
      ".jf-progress-container {" +
      "  width: 200px;" +
      "  height: 6px;" +
      "  display: flex;" +
      "  align-items: center;" +
      "  position: relative;" +
      "}" +
      ".jf-progress-bar {" +
      "  height: 5px;" +
      "  background: " + config.progressBarColor + ";" +
      "  border-radius: 2px;" +
      "  transition: width 0.2s ease-in-out;" +
      "}" +
      ".jf-progress-gap {" +
      "  width: 6px;" +
      "  height: 5px;" +
      "  background: transparent;" +
      "  flex-shrink: 0;" +
      "}" +
      ".jf-unfilled-bar {" +
      "  height: 5px;" +
      "  background: rgba(128, 128, 128, 0.5);" +
      "  border-radius: 2px;" +
      "  flex-grow: 1;" +
      "  transition: width 0.2s ease-in-out;" +
      "}";
    document.head.appendChild(style);

    var loadingDiv = document.createElement("div");
    loadingDiv.className = "bar-loading";
    loadingDiv.id = "jf-page-loader";

    var messageHtml = config.loadingMessage
      ? '<div class="jf-loader-message">' + escapeHtml(config.loadingMessage) + "</div>"
      : "";

    loadingDiv.innerHTML =
      '<div class="jf-loader-content">' +
      "<h1><div class=\"jf-splash-logo\"" +
      (config.logoUrl ? ' style="background-image: url(\'' + config.logoUrl + '\')"' : "") +
      "></div></h1>" +
      messageHtml +
      '<div class="jf-progress-container">' +
      '<div class="jf-progress-bar" id="jf-progress-bar"></div>' +
      '<div class="jf-progress-gap"></div>' +
      '<div class="jf-unfilled-bar" id="jf-unfilled-bar"></div>' +
      "</div>" +
      "</div>";

    document.body.appendChild(loadingDiv);

    requestAnimationFrame(function () {
      var logo = loadingDiv.querySelector(".jf-splash-logo");
      if (logo) {
        logo.style.opacity = "1";
      }
    });

    var progressBar = document.getElementById("jf-progress-bar");
    var unfilledBar = document.getElementById("jf-unfilled-bar");

    var progress = 0;
    var lastIncrement = 5;

    var progressInterval = setInterval(function () {
      if (progress < 95) {
        lastIncrement = Math.max(0.5, lastIncrement * 0.98);
        var randomFactor = 0.8 + Math.random() * 0.4;
        progress = Math.min(progress + lastIncrement * randomFactor, 95);
        progressBar.style.width = progress + "%";
        unfilledBar.style.width = 100 - progress + "%";
      }
    }, 150);

    var finished = false;

    function finishLoading() {
      if (finished) {
        return;
      }
      finished = true;
      clearInterval(progressInterval);
      clearInterval(checkInterval);

      var elapsed = Date.now() - startTime;
      var minMs = config.minimumDisplayDurationMs || 0;
      var wait = Math.max(0, minMs - elapsed);

      setTimeout(function () {
        progressBar.style.width = "100%";
        unfilledBar.style.width = "0%";
        setTimeout(function () {
          loadingDiv.classList.add("hide");
          setTimeout(function () {
            loadingDiv.remove();
            style.remove();
          }, 300);
        }, 150);
      }, wait);
    }

    var checkInterval = setInterval(function () {
      var loginFormLoaded = document.querySelector(".manualLoginForm");
      var activeTab = document.querySelector(".pageTabContent.is-active");

      if (loginFormLoaded || activeTab) {
        finishLoading();
      }
    }, 100);
  }

  function escapeHtml(value) {
    var div = document.createElement("div");
    div.textContent = value;
    return div.innerHTML;
  }
})();
