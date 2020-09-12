import Oidc from "oidc-client";

export function initOidc() {
    var config = {
        authority: "https://demo.identityserver.io",
        client_id: "interactive.public.short",
        redirect_uri: `${window.location.origin}/callback.html`,
        response_type: "code",
        scope: "openid profile api offline_access",
        clockSkew: 15,
        automaticSilentRenew: true,
        silent_redirect_uri: `${window.location.origin}/callback-silent.html`,
        accessTokenExpiringNotificationTime: 0,
        monitorAnonymousSession: true,
    };
    var mgr = new Oidc.UserManager(config);
    window.OidcManager = mgr;
    mgr.getUser().then(user => {
        if (!user || user.expired) {
            mgr.signinSilent()
                .then(user => { })
                .catch(ex => {
                    console.log(ex);
                    mgr.signinRedirect()
                });
        }
    });
}

export function requestInterceptor(request) {
    return window.OidcManager.getUser().then(function (user) {
        if (!user) {
            return request;
        }
        request.headers.authorization = 'Bearer ' + user.access_token;
        return request;
    });
};

initOidc();