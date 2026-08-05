# Source material

The design spec and implementation plans for this site carry a standing constraint: **every
biographical or career claim on the site must trace to source material, with nothing invented.**

That source material is deliberately **not committed to this repository.**

## Why not

This repository is public. The resume drafts contain a phone number and the personal-details file
contains a street address. The site's own design spec (`docs/superpowers/specs/`) decided that
jasonobject.work publishes email, GitHub and LinkedIn but **no phone number and no street
address** — committing the sources would publish exactly that by the back door, and git history
would keep it available in every clone even after a later deletion.

## Where it lives

```
/home/jason/sites/medotnet-source/
├── Jason_StJohn_Resume_AI_Angle.txt
├── Jason_StJohn_Resume_Standard.txt
├── Resume_Raw_Materials.txt
└── Personal_info.txt
```

Outside the working tree, on the same machine. Image originals live in `ImagePool/` in the repo
root, which is gitignored for size reasons (~15 MB of unprocessed camera files); only the derived,
resized images under `src/MeDotNet/wwwroot/img/` are committed.

## Consequence

The traceability constraint cannot be independently verified by anyone reading this repository
alone. That is an accepted trade: the alternative is publishing personal contact details the site
itself deliberately withholds.

If these files move, update this document.
