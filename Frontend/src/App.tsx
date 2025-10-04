import { Link } from 'react-router-dom';

type FeatureCard = {
  title: string;
  description: string;
  to: string;
};

const featureCards: FeatureCard[] = [
  {
    title: 'TypeScript Ready',
    description: 'Enjoy full type safety and editor tooling out of the box.',
    to: '/docs/typescript'
  },
  {
    title: 'Tailwind Styling',
    description: 'Compose modern UI with utility classes and responsive design.',
    to: '/docs/tailwind'
  },
  {
    title: 'Routing Built In',
    description: 'Start wiring pages with React Router v6 navigation primitives.',
    to: '/docs/router'
  }
];

const App = () => {
  return (
    <div className="min-h-screen bg-slate-950 text-slate-100">
      <div className="mx-auto flex w-full max-w-5xl flex-col gap-16 px-4 py-16">
        <header className="flex flex-col gap-6 text-center">
          <span className="mx-auto rounded-full border border-sky-500/30 bg-sky-500/10 px-4 py-1 text-xs font-semibold uppercase tracking-[0.3em] text-sky-300">
            HackYeah 2025
          </span>
          <div className="mx-auto max-w-2xl space-y-4">
            <h1 className="text-4xl font-semibold leading-tight sm:text-5xl">
              Frontend starter with TypeScript, React Router, and Tailwind
            </h1>
            <p className="text-base text-slate-300 sm:text-lg">
              You are ready to build rich interfaces faster. Explore the starter docs below or jump straight into creating new routes and components.
            </p>
          </div>
          <div className="flex flex-wrap items-center justify-center gap-3">
            <Link
              to="/getting-started"
              className="rounded-lg bg-sky-500 px-5 py-2 text-sm font-semibold text-slate-950 shadow-lg shadow-sky-500/30 transition hover:bg-sky-400 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-sky-400"
            >
              View Getting Started Guide
            </Link>
            <a
              href="https://react.dev/"
              target="_blank"
              rel="noreferrer"
              className="rounded-lg border border-slate-700 px-5 py-2 text-sm font-semibold text-slate-200 transition hover:border-slate-500 hover:text-white focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-slate-400"
            >
              React Documentation
            </a>
          </div>
        </header>

        <section className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
          {featureCards.map(card => (
            <article
              key={card.title}
              className="group flex h-full flex-col justify-between rounded-2xl border border-slate-800 bg-slate-900/60 p-6 shadow-lg shadow-black/20 transition hover:border-sky-500/60 hover:bg-slate-900"
            >
              <div className="space-y-3">
                <h2 className="text-lg font-semibold text-white">
                  {card.title}
                </h2>
                <p className="text-sm text-slate-300">{card.description}</p>
              </div>
              <Link
                to={card.to}
                className="mt-6 inline-flex items-center gap-2 text-sm font-semibold text-sky-300 transition group-hover:text-sky-200"
              >
                Explore
                <span aria-hidden="true" className="text-lg">
                  →
                </span>
              </Link>
            </article>
          ))}
        </section>
      </div>
    </div>
  );
};

export default App;
