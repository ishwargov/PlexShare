# work flow

* disclaimer: this is what i think after discussion with joel and ish pls put corrections in grp if you feel this is wrong



## steps

1. fork my repo
* go to https://github.com/jerryJohnThomas/plexshare
* and in the top you will see fork option
* click on that
* now you will have a forked repo in your github https://github.com/your_github_id/plexshare
* next part:
    * git clone your_repo
    * `git remote add upstream `
    * `git remote add upstream https://github.com/jerryJohnThomas/plexshare`
    * to make sure that worked do `git remote -v` on which you should see 4 things, 2 with your name and 2 with jerry name.
    * to do future updates (not necessary for today)
        * to make your repo always upto_date
        * `git fetch upstream`
        * `git checkout main`
        * `git merge upstream/main`
        * now your main will be same as my main.

2. add your changes to your repo
* now inside whiteboard folder specs you create your spec
* put photos inside assets folder pls follow unique naming asha_pic1 or somthing like that to prevent overlap
* or if you have a better idea for pics pls suggest in grp 


3. git add , git commit git push your changes


4. pull request to my repo
* go to my repo : https://github.com/jerryJohnThomas/plexshare
* go to pull requests
*  new pull request
* then basically i think select your repo or something like that
* add some text or message
* click on create pull request


video_ref - https://www.youtube.com/watch?v=a_FLqX3vGR4



****
