﻿namespace WebSharper.UI

open WebSharper
open WebSharper.UI.Html
open WebSharper.UI.Client
open WebSharper.UI.Notation

// A small example of a single-page mini-sitelet.
// See this live at http://intellifactory.github.io/websharper.ui.next/#MiniSiteletTest.fs !

[<JavaScript>]
module AnimatedBobsleighSite =

    // Page is a record that we use to represent the current page we're on.
    // Notice that this doesn't specify anything about the rendering of the
    // page. That comes later.
    type Page =
        | BobsleighHome
        | BobsleighHistory
        | BobsleighGovernance
        | BobsleighTeam

    // Sets the variable to a given action, changing the page. This can be
    // called from anywhere.
    let GlobalGo var (act : Page) =
        Var.Set var act

    // Gives a textual representation of a Page type. We use this in the Navabr.
    let showAct = function
        | BobsleighHome -> "Home"
        | BobsleighHistory -> "History"
        | BobsleighGovernance -> "Governance"
        | BobsleighTeam -> "The IntelliFactory Bobsleigh Team"

    // A list of pages in the site
    let pages = [ BobsleighHome ; BobsleighHistory ; BobsleighGovernance ; BobsleighTeam ]

    // A record containing a continuation function we can use within pages.
    type Context = {
        Go : Page -> unit
    }

    // A fade animation
    let fadeTime = 300.0
    let Fade =
        Anim.Simple Interpolation.Double Easing.CubicInOut fadeTime

    let FadeTransition =
        Trans.Create Fade
        |> Trans.Enter (fun i -> Fade 0.0 1.0)
        |> Trans.Exit (fun i -> Fade 1.0 0.0)

    // Creates a navigation bar using the variable containing the current page.
    let NavBar var =
        View.FromVar var
        |> View.Map (fun active ->

            let renderLink action =
                let attr = if action = active then cls "active" else Attr.Empty

                li [attr] [
                    Doc.Link (showAct action) [] (fun _ -> GlobalGo var action)
                ] :> Doc

            nav [cls "navbar navbar-default" ; Attr.Create "role" "navigation"] [
                ul [cls "nav navbar-nav"] [
                    List.map renderLink pages |> Doc.Concat
                ]
            ])
        |> Doc.EmbedView

    let MakePage var pg =
        Doc.Concat [
            NavBar var
            div [Attr.AnimatedStyle "opacity" FadeTransition (View.Const 1.0) string] [
                pg
            ]
        ]

    // Each page takes the context record we created earlier. We could also just have it
    // taking the continuation function.
    // To change the page, we call this function with the action we want to perform.
    let HomePage ctx =
        Doc.Concat [
            div [] [
                h1 [] [text "Welcome!"]
                p [] [
                    text "Welcome to the IntelliFactory Bobsleigh MiniSite!"
                ]
                p [] [
                    text "Here you can find out about the "
                    Doc.Link "history" [] (fun () -> ctx.Go BobsleighHistory)
                    text " of bobsleighs, the "
                    Doc.Link "International Bobsleigh and Skeleton Federation" [] (fun () -> ctx.Go BobsleighGovernance)
                    text ", which serve as the governing body for the sport, and finally the world-famous "
                    Doc.Link "IntelliFactory Bobsleigh Team." [] (fun () -> ctx.Go BobsleighTeam)
                ]
            ]
        ]

    let History ctx =
        Doc.Concat [
            div [] [
                h1 [] [text "History"]
                p [] [
                    text "According to "
                    href "Wikipedia" "http://en.wikipedia.org/wiki/Bobsleigh"
                    text ", the beginnings of bobsleigh came about due to a hotelier \
                         becoming increasingly frustrated about having entire seasons \
                         where he could not rent out his properties. In response, he got \
                         a few people interested, and the Swiss town of St Moritz became \
                         the home of the first bobsleigh races."
                ]
                p [] [
                    text "Bobsleigh races have been a regular event at the Winter Olympics \
                         since the very first competition in 1924."
                ]
            ]
        ]

    let Governance ctx =
        Doc.Concat [
            div [] [
                h1 [] [text "Governance"]
                p [] [
                    text "The sport is overseen by the "
                    href "International Bobsleigh and Skeleton Federation" "http://www.fibt.com/"
                    text ", an organisation founded in 1923. \
                         The organisation governs all international competitions, \
                         acting as a body to regulate athletes' conduct, as well as \
                         providing funding for training and education."
                ]
            ]
        ]

    let Team ctx =
        let teamMembers =
            [("Adam", "granicz") ; ("András", "AndrasJanko") ; ("Anton (honourary member for life)", "t0yv0") ;
             ("István", "inchester23") ; ("Loic", "tarmil_") ; ("Sándor", "sandorrakonczai")
             ("Simon", "Simon_JF")]

        Doc.Concat [
            div [] [
                h1 [] [text "The IntelliFactory Bobsleigh Team"]
                p [] [
                    text "The world-famous IntelliFactory Bobsleigh Team was founded \
                         in 2004, and currently consists of:"
                ]
                ul [] [
                    teamMembers
                    |> List.map (fun (name, handle) ->
                        li [] [href name ("http://www.twitter.com/" + handle)] :> Doc)
                    |> Doc.Concat
                ]
            ]
        ]

    // Here we define the sitelet.
    let Main _ =
        // We first create a variable to hold our current page.
        let m = Var.Create BobsleighHome

        // Here, we create the sitelet. We supply the Create function
        // with the variable we created, and we get a continuation function,
        // go, which we can use to change the page.
       // MiniSitelet.Create m (fun go ->
        let ctx = {Go = Var.Set m}
        View.FromVar m
        |> View.Map (fun pg ->
            match pg with
            | BobsleighHome -> HomePage ctx
            | BobsleighHistory -> History ctx
            | BobsleighGovernance -> Governance ctx
            | BobsleighTeam -> Team ctx
            |> MakePage m)
        |> Doc.EmbedView

    let description _ =
        div [] [
            text "A small website about bobsleighs, demonstrating how UI \
                 may be used to structure single-page applications."
        ]

    // You can ignore the bits here -- it just links the example into the site.
    let Sample =
        Samples.Build(Samples.AnimatedBobsleighSite)
            .Id("AnimatedBobsleighSite")
            .FileName(__SOURCE_FILE__)
            .Keywords(["text"])
            .Render(Main)
            .RenderDescription(description)
            .Create()
