import {ICompileService, IDirective, IQService} from "angular";
import {AppFooterController} from "./app-footer.controller";

/* @ngInject */
export class AppFooterDirective implements IDirective {

    //#region Properties

    // Directive restriction.
    restrict = 'E';


    // Compile directive lazily.
    compile = () => {
        let pGetTemplatePromise = this.$q((resolve) => {
            require.ensure([], () => {
                require('./app-footer.scss');
                resolve(require('./app-footer.html'))
            });
        });
        return (scope, element) => {
            pGetTemplatePromise
                .then((htmlTemplate) => {
                    element.html(htmlTemplate);
                    this.$compile(element.contents())(scope)
                });
        };
    };

    // Directive controller.
    controller = AppFooterController;

    //#endregion

    //#region Constructor

    // Initialize directive with injectors.
    public constructor(public $q: IQService, public $compile: ICompileService) {


    }

    //#endregion
}