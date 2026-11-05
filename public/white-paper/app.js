/**
 * Google Mind the Gap — Interactive Application Logic
 * Integrates Apple / Meta / Anthropic UI behaviors:
 * - Animated Counters
 * - 4-Step Architecture Carousel & Stepper
 * - Problem vs Solution Interactive Filter
 * - HelloTech GenAI Prompt Sandbox Simulator
 * - Stakeholder Persona Tab Switcher
 * - Modal Booking Flows & Toast Notifications
 * - Theme Switcher (Dark / Light) fixed at bottom right
 * - Fluid Motion Sliding Blue Nav Pill Indicator
 */

document.addEventListener('DOMContentLoaded', () => {

  // =========================================================================
  // 1. Theme Management (Dark / Light)
  // =========================================================================
  const themeToggleBtn = document.getElementById('theme-toggle');
  const htmlRoot = document.documentElement;

  // Retrieve saved theme or default to dark
  const savedTheme = localStorage.getItem('google_mtg_theme') || 'dark';
  htmlRoot.setAttribute('data-theme', savedTheme);

  if (themeToggleBtn) {
    themeToggleBtn.addEventListener('click', () => {
      const currentTheme = htmlRoot.getAttribute('data-theme') || 'dark';
      const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
      htmlRoot.setAttribute('data-theme', newTheme);
      localStorage.setItem('google_mtg_theme', newTheme);
      showToast(`Switched to ${newTheme} mode`);
    });
  }

  // =========================================================================
  // 2. Fluid Motion Sliding Blue Pill Nav Indicator (Requirement 4)
  // =========================================================================
  const sliderPill = document.getElementById('nav-slider-pill');
  const navLinks = document.querySelectorAll('.nav-link');
  const navWrapper = document.querySelector('.nav-links-wrapper');
  const brandBadge = document.getElementById('brand-badge');

  const updateNavIndicator = (activeElement) => {
    if (!sliderPill || !navWrapper || !activeElement) return;

    const wrapperRect = navWrapper.getBoundingClientRect();
    const elemRect = activeElement.getBoundingClientRect();

    const leftOffset = elemRect.left - wrapperRect.left;
    const topOffset = elemRect.top - wrapperRect.top;
    const width = elemRect.width;
    const height = elemRect.height;

    sliderPill.style.opacity = '1';
    sliderPill.style.width = `${width}px`;
    sliderPill.style.height = `${height}px`;
    sliderPill.style.transform = `translate3d(${leftOffset}px, ${topOffset}px, 0)`;
  };

  // Nav link click event
  navLinks.forEach(link => {
    link.addEventListener('click', (e) => {
      navLinks.forEach(l => l.classList.remove('active'));
      link.classList.add('active');
      updateNavIndicator(link);
    });
  });

  // Track sections on scroll to slide the blue indicator fluidly
  const sectionIds = [
    'sec-executive-summary',
    'sec-opportunity-gap',
    'sec-origins-thesis',
    'sec-program-architecture',
    'sec-coalition',
    'sec-strategic-impact',
    'sec-how-to-get-involved',
    'sec-works-cited'
  ];

  const sections = sectionIds.map(id => document.getElementById(id)).filter(Boolean);

  const handleScrollNav = () => {
    const scrollPos = window.scrollY + 140;

    // Toggle .is-scrolled class to remove any residual styling on brand badge
    if (window.scrollY >= 100) {
      document.body.classList.add('is-scrolled');
    } else {
      document.body.classList.remove('is-scrolled');
    }

    // If near the top (hero), indicator sits around brand badge "White Paper"
    if (window.scrollY < 120) {
      navLinks.forEach(l => l.classList.remove('active'));
      if (brandBadge && sliderPill && navWrapper) {
        // Fluid transition around brand badge at the top
        const badgeRect = brandBadge.getBoundingClientRect();
        const wrapperRect = navWrapper.getBoundingClientRect();
        const leftOffset = badgeRect.left - wrapperRect.left;
        const topOffset = badgeRect.top - wrapperRect.top;
        sliderPill.style.opacity = '0.9';
        sliderPill.style.width = `${badgeRect.width}px`;
        sliderPill.style.height = `${badgeRect.height}px`;
        sliderPill.style.transform = `translate3d(${leftOffset}px, ${topOffset}px, 0)`;
      } else if (sliderPill) {
        sliderPill.style.opacity = '0';
      }
      return;
    }

    let activeFound = false;
    for (let i = sections.length - 1; i >= 0; i--) {
      const section = sections[i];
      if (section && section.offsetTop <= scrollPos) {
        const activeLink = document.querySelector(`.nav-link[data-nav-target="${section.id}"]`);
        if (activeLink) {
          navLinks.forEach(l => l.classList.remove('active'));
          activeLink.classList.add('active');
          updateNavIndicator(activeLink);
          activeFound = true;
          break;
        }
      }
    }

    // When scrolled down to Summary, cleanly slide blue indicator over Summary
    if (!activeFound && navLinks.length > 0 && window.scrollY >= 120) {
      navLinks.forEach(l => l.classList.remove('active'));
      navLinks[0].classList.add('active');
      updateNavIndicator(navLinks[0]);
    }
  };

  window.addEventListener('scroll', handleScrollNav, { passive: true });
  window.addEventListener('resize', handleScrollNav, { passive: true });
  setTimeout(handleScrollNav, 200);

  // =========================================================================
  // 3. Animated Metrics Counters (IntersectionObserver)
  // =========================================================================
  const counters = document.querySelectorAll('.counter');
  let countersAnimated = false;

  const animateCounters = () => {
    counters.forEach(counter => {
      const target = +counter.getAttribute('data-target');
      const duration = 1800; // ms
      const startTime = performance.now();

      const updateCount = (currentTime) => {
        const elapsed = currentTime - startTime;
        const progress = Math.min(elapsed / duration, 1);
        
        // Easing function (easeOutCubic)
        const easeOutProgress = 1 - Math.pow(1 - progress, 3);
        const currentValue = Math.floor(easeOutProgress * target);

        if (target >= 1000) {
          counter.textContent = currentValue.toLocaleString();
        } else {
          counter.textContent = currentValue;
        }

        if (progress < 1) {
          requestAnimationFrame(updateCount);
        } else {
          counter.textContent = target >= 1000 ? target.toLocaleString() : target;
        }
      };

      requestAnimationFrame(updateCount);
    });
  };

  const metricsSection = document.getElementById('metrics');
  if (metricsSection) {
    const observer = new IntersectionObserver((entries) => {
      entries.forEach(entry => {
        if (entry.isIntersecting && !countersAnimated) {
          countersAnimated = true;
          animateCounters();
        }
      });
    }, { threshold: 0.3 });

    observer.observe(metricsSection);
  }

  // =========================================================================
  // 4. Problem vs Solution Interactive Toggle
  // =========================================================================
  const splitTabs = document.querySelectorAll('.split-tab-btn');
  const splitGrid = document.getElementById('split-grid');
  const problemCard = document.querySelector('.problem-card');
  const solutionCard = document.querySelector('.solution-card');

  splitTabs.forEach(tab => {
    tab.addEventListener('click', () => {
      splitTabs.forEach(t => t.classList.remove('active'));
      tab.classList.add('active');

      const mode = tab.getAttribute('data-split-mode');
      if (mode === 'side-by-side') {
        splitGrid.style.gridTemplateColumns = '1fr 1fr';
        problemCard.style.display = 'flex';
        solutionCard.style.display = 'flex';
      } else if (mode === 'problem-only') {
        splitGrid.style.gridTemplateColumns = '1fr';
        problemCard.style.display = 'flex';
        solutionCard.style.display = 'none';
      } else if (mode === 'solution-only') {
        splitGrid.style.gridTemplateColumns = '1fr';
        problemCard.style.display = 'none';
        solutionCard.style.display = 'flex';
      }
    });
  });

  // =========================================================================
  // 5. Program Architecture Stepper & Carousel
  // =========================================================================
  const stepButtons = document.querySelectorAll('.step-btn');
  const stepSlides = document.querySelectorAll('.step-slide');
  const stepDots = document.querySelectorAll('.step-dot');
  const prevStepBtn = document.getElementById('prev-step-btn');
  const nextStepBtn = document.getElementById('next-step-btn');
  let currentStep = 0;
  const totalSteps = stepButtons.length;

  const setStep = (index) => {
    currentStep = (index + totalSteps) % totalSteps;

    stepButtons.forEach((btn, idx) => {
      const isActive = idx === currentStep;
      btn.classList.toggle('active', isActive);
      btn.setAttribute('aria-selected', isActive);
    });

    stepSlides.forEach((slide, idx) => {
      slide.classList.toggle('active', idx === currentStep);
    });

    stepDots.forEach((dot, idx) => {
      dot.classList.toggle('active', idx === currentStep);
    });
  };

  stepButtons.forEach((btn) => {
    btn.addEventListener('click', () => {
      const stepIndex = parseInt(btn.getAttribute('data-step'), 10);
      setStep(stepIndex);
    });
  });

  stepDots.forEach((dot) => {
    dot.addEventListener('click', () => {
      const dotIndex = parseInt(dot.getAttribute('data-index'), 10);
      setStep(dotIndex);
    });
  });

  if (prevStepBtn) {
    prevStepBtn.addEventListener('click', () => setStep(currentStep - 1));
  }
  if (nextStepBtn) {
    nextStepBtn.addEventListener('click', () => setStep(currentStep + 1));
  }

  // =========================================================================
  // 6. Interactive HelloTech GenAI Sandbox Simulator
  // =========================================================================
  const promptInput = document.getElementById('prompt-input');
  const runSimBtn = document.getElementById('run-simulation-btn');
  const resetPromptBtn = document.getElementById('reset-prompt-btn');
  const outputText = document.getElementById('output-text');
  const simStatus = document.getElementById('sim-status');
  const presetButtons = document.querySelectorAll('.preset-btn');

  const presets = {
    community: {
      prompt: "Act as an educational AI assistant for 9th graders. Analyze school energy consumption data and propose three collaborative mini-projects students can build using visual code.",
      output: `✨ [Gemini 1.5 Education Safe Engine]

Great! Based on typical secondary school electrical draw, here are three collaborative student project concepts:

1. ⚡ "Smart Cafeteria Light Patrol": A visual sensor block program that triggers alerts when natural daylight suffices, calculating cumulative energy savings.
2. 🌡️ "HVAC Micro-Zone Tracker": Students log classroom comfort scores against thermostat data to predict waste patterns using elementary decision trees.
3. 📱 "Solar Rooftop Visualizer": A clean interactive web widget allowing students to simulate adding 20 solar panels and measuring CO2 offsets in real time.

Student Efficacy Check: Visual coding bridge verified. No syntax hurdles; immediate algorithmic empowerment.`,
      stats: { latency: "138ms", toxicity: "0.00%", belonging: "99/100" }
    },
    ethics: {
      prompt: "Audit this image classifier trained to recognize 'scientists'. Evaluate whether historical training data exhibits gender or cultural bias and suggest guardrails.",
      output: `🛡️ [Ethical AI Guardrail Module]

Bias Audit Result:
• Detected Skew: Historical dataset contains 78% imagery of male scientists in lab coats.
• Risk Factor: Model may produce false negatives for female and non-traditional researchers in applied software and field sciences.

Proposed Student Guardrails:
1. Counter-factual data augmentation: Include equal representations of women leading AI, robotics, and biology initiatives.
2. Demographic parity test: Flag any prediction with disproportionate confidence shifts across gender categories.
3. Transparent confidence badge: Display certainty metrics directly to the user.`,
      stats: { latency: "154ms", toxicity: "0.00%", belonging: "98/100" }
    },
    vision: {
      prompt: "Design a camera logic rule to help students with visual impairments navigate school hallways safely.",
      output: `👁️ [Computer Vision Assistive Module]

Architecture Blueprint:
• Input: Real-time 30fps webcam feed processed via lightweight on-device TensorFlow.js.
• Logic Rule: If depth sensor detects an obstruction < 1.5 meters ahead, translate bounding box coordinate into directional spatial audio ("Obstacle 2 feet to your left").
• Privacy Guardrail: All face detection features are disabled at the hardware layer to safeguard student biometric privacy.`,
      stats: { latency: "112ms", toxicity: "0.00%", belonging: "100/100" }
    }
  };

  if (promptInput) {
    promptInput.value = presets.community.prompt;
  }

  presetButtons.forEach(btn => {
    btn.addEventListener('click', () => {
      presetButtons.forEach(b => b.classList.remove('active'));
      btn.classList.add('active');
      const presetKey = btn.getAttribute('data-preset');
      if (presets[presetKey]) {
        promptInput.value = presets[presetKey].prompt;
        showToast(`Loaded ${btn.querySelector('strong').textContent} challenge`);
      }
    });
  });

  if (runSimBtn) {
    runSimBtn.addEventListener('click', () => {
      const activePresetBtn = document.querySelector('.preset-btn.active');
      const activeKey = activePresetBtn ? activePresetBtn.getAttribute('data-preset') : 'community';
      const presetData = presets[activeKey] || presets.community;

      runSimBtn.disabled = true;
      simStatus.textContent = "Status: Executing model inference...";
      outputText.innerHTML = '<span class="pulse-indicator"></span> <em>Running HelloTech student simulation...</em>';

      setTimeout(() => {
        runSimBtn.disabled = false;
        simStatus.textContent = "Status: Execution complete (200 OK)";
        outputText.innerHTML = presetData.output.replace(/\n/g, '<br />');

        const statsRow = document.getElementById('output-stats');
        if (statsRow) {
          statsRow.innerHTML = `
            <span class="stat-tag">Latency: ${presetData.stats.latency}</span>
            <span class="stat-tag">Toxicity: ${presetData.stats.toxicity}</span>
            <span class="stat-tag">Belonging Score: ${presetData.stats.belonging}</span>
          `;
        }
        showToast("Workshop run successfully generated!");
      }, 700);
    });
  }

  if (resetPromptBtn) {
    resetPromptBtn.addEventListener('click', () => {
      const activePresetBtn = document.querySelector('.preset-btn.active');
      const activeKey = activePresetBtn ? activePresetBtn.getAttribute('data-preset') : 'community';
      promptInput.value = presets[activeKey].prompt;
      outputText.textContent = 'Click "Execute AI Workshop Run" to simulate the student hands-on coding experience.';
      simStatus.textContent = "Status: Ready to run";
    });
  }

  // =========================================================================
  // 7. Stakeholder Persona Tabs Switcher
  // =========================================================================
  const personaButtons = document.querySelectorAll('.persona-nav-btn');
  const personaPanels = document.querySelectorAll('.persona-panel');

  personaButtons.forEach(btn => {
    btn.addEventListener('click', () => {
      personaButtons.forEach(b => {
        b.classList.remove('active');
        b.setAttribute('aria-selected', 'false');
      });
      btn.classList.add('active');
      btn.setAttribute('aria-selected', 'true');

      const targetPersona = btn.getAttribute('data-persona');
      personaPanels.forEach(panel => {
        const isTarget = panel.id === `panel-${targetPersona}`;
        panel.classList.toggle('active', isTarget);
      });
    });
  });

  // =========================================================================
  // 8. Modal Registration Dialog
  // =========================================================================
  const registerModal = document.getElementById('register-modal');
  const openModalBtns = [
    document.getElementById('open-register-modal-btn'),
    document.getElementById('hero-register-btn'),
    ...document.querySelectorAll('.open-booking-trigger')
  ].filter(Boolean);

  const closeModalBtn = document.getElementById('close-modal-btn');
  const cancelModalBtn = document.getElementById('cancel-modal-btn');
  const cohortForm = document.getElementById('cohort-form');
  const modalSuccess = document.getElementById('modal-success');
  const successDoneBtn = document.getElementById('success-done-btn');

  const openModal = () => {
    if (registerModal) {
      registerModal.classList.add('open');
      registerModal.setAttribute('aria-hidden', 'false');
      document.body.style.overflow = 'hidden';
      if (cohortForm) cohortForm.style.display = 'flex';
      if (modalSuccess) modalSuccess.style.display = 'none';
    }
  };

  const closeModal = () => {
    if (registerModal) {
      registerModal.classList.remove('open');
      registerModal.setAttribute('aria-hidden', 'true');
      document.body.style.overflow = '';
    }
  };

  openModalBtns.forEach(btn => btn?.addEventListener('click', openModal));
  closeModalBtn?.addEventListener('click', closeModal);
  cancelModalBtn?.addEventListener('click', closeModal);
  successDoneBtn?.addEventListener('click', closeModal);

  registerModal?.addEventListener('click', (e) => {
    if (e.target === registerModal) closeModal();
  });

  document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape' && registerModal?.classList.contains('open')) {
      closeModal();
    }
  });

  cohortForm?.addEventListener('submit', (e) => {
    e.preventDefault();
    const schoolName = document.getElementById('school-name')?.value || 'School';
    cohortForm.style.display = 'none';
    if (modalSuccess) {
      modalSuccess.style.display = 'block';
    }
    showToast(`Application submitted for ${schoolName}!`);
  });

  const partnerInquiryBtn = document.getElementById('partner-inquiry-btn');
  partnerInquiryBtn?.addEventListener('click', () => {
    openModal();
    const notesField = document.getElementById('notes');
    if (notesField) {
      notesField.value = "Corporate / Non-Profit Partnership Inquiry: Interested in co-hosting regional summits and curriculum integration.";
    }
  });

  // =========================================================================
  // 9. FAQ Accordion Toggle
  // =========================================================================
  const faqItems = document.querySelectorAll('.faq-item');
  faqItems.forEach(item => {
    const questionBtn = item.querySelector('.faq-question');
    questionBtn?.addEventListener('click', () => {
      const wasActive = item.classList.contains('active');
      faqItems.forEach(i => i.classList.remove('active'));
      if (!wasActive) {
        item.classList.add('active');
      }
    });
  });

  // =========================================================================
  // 10. Toast Notification System
  // =========================================================================
  function showToast(message) {
    const container = document.getElementById('toast-container');
    if (!container) return;

    const toast = document.createElement('div');
    toast.className = 'toast';
    toast.innerHTML = `
      <svg viewBox="0 0 24 24" width="16" height="16" fill="currentColor" style="color: var(--google-blue);">
        <circle cx="12" cy="12" r="10"/>
        <polyline points="12 6 12 12 14 14" stroke="#ffffff" stroke-width="2" fill="none"/>
      </svg>
      <span>${message}</span>
    `;

    container.appendChild(toast);

    setTimeout(() => {
      toast.style.opacity = '0';
      toast.style.transform = 'translateY(10px)';
      toast.style.transition = 'all 0.3s ease';
      setTimeout(() => toast.remove(), 300);
    }, 3200);
  }

});
